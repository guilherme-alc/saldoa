"""Render Coverlet's Cobertura XML as a GitHub summary and a standalone HTML report."""

import argparse
from html import escape
from pathlib import Path
import xml.etree.ElementTree as ET


REPOSITORY = Path(__file__).resolve().parents[2]

HTML_HEADER = """<!doctype html>
<html lang="en">
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>Test coverage</title>
<style>
  :root { color-scheme: light dark; }
  body { font: 1rem system-ui, sans-serif; margin: 2rem auto; max-width: 72rem; padding: 0 1rem; }
  details { margin: .6rem 0; border: 1px solid GrayText; border-radius: .3rem; padding: .6rem; }
  summary { cursor: pointer; overflow-wrap: anywhere; }
  table { border-collapse: collapse; width: 100%; margin-top: .8rem; }
  th, td { padding: .25rem .5rem; text-align: left; vertical-align: top; }
  td:first-child { border-left: .3rem solid #238636; }
  .missed td:first-child { border-left-color: #da3633; }
  code { white-space: pre-wrap; overflow-wrap: anywhere; }
</style>
<main>
<h1>Test coverage</h1>
<p>Open a class to see its measured lines. Zero hits means the test suite did not execute that line.</p>
"""


def source_lines(filename, sources):
    for source in sources:
        path = (REPOSITORY / source / filename).resolve()
        if path.is_relative_to(REPOSITORY) and path.is_file():
            return path.read_text(encoding="utf-8-sig", errors="replace").splitlines()
    return []


def render_class(coverage_class, name, line_rate, branch_rate, lines, sources):
    filename = coverage_class.attrib["filename"]
    source = source_lines(filename, sources)
    rows = []
    for line in sorted(lines, key=lambda line: int(line.attrib["number"])):
        number = int(line.attrib["number"])
        hits = int(line.attrib["hits"])
        branches = escape(line.get("condition-coverage", ""))
        code = escape(source[number - 1]) if 0 < number <= len(source) else ""
        row_class = "covered" if hits else "missed"
        rows.append(
            f'<tr class="{row_class}"><td>{number}</td><td>{hits}</td>'
            f"<td>{branches}</td><td><code>{code}</code></td></tr>"
        )
    return (
        f"<details><summary>{name} · {line_rate} lines · {branch_rate} branches</summary>"
        f"<p><code>{escape(filename)}</code></p>"
        "<table><thead><tr><th>Line</th><th>Hits</th><th>Branches</th><th>Source</th></tr></thead>"
        f"<tbody>{''.join(rows)}</tbody></table></details>\n"
    )


def format_total(coverage, metric):
    covered = int(coverage.attrib[f"{metric}-covered"])
    total = int(coverage.attrib[f"{metric}-valid"])
    return f"{covered}/{total} ({covered / total:.1%})" if total else "—"


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("results_dir", type=Path)
    results_dir = parser.parse_args().results_dir
    reports = sorted(results_dir.rglob("coverage.cobertura*.xml"))
    if not reports:
        parser.error(f"No Cobertura XML report found in {results_dir}")

    summary = ["## Test coverage\n\n"]
    html = [HTML_HEADER]
    for report in reports:
        coverage = ET.parse(report).getroot()
        if len(reports) > 1:
            report_name = escape(report.relative_to(results_dir).as_posix())
            summary.append(f"### {report_name}\n\n")
            html.append(f"<h2>{report_name}</h2>\n")

        lines = format_total(coverage, "lines")
        branches = format_total(coverage, "branches")
        summary.append(f"**Lines** {lines} · **Branches** {branches}\n\n")
        summary.append("| Class | Lines | Branches |\n| --- | ---: | ---: |\n")
        html.append(f"<p><strong>Lines</strong> {lines} · <strong>Branches</strong> {branches}</p>\n")

        sources = [source.text for source in coverage.findall("./sources/source") if source.text]
        classes = coverage.findall("./packages/package/classes/class")
        for coverage_class in sorted(classes, key=lambda entry: entry.attrib["name"]):
            name = escape(coverage_class.attrib["name"])
            measured_lines = coverage_class.findall("./lines/line")
            line_rate = f"{float(coverage_class.attrib['line-rate']):.1%}"
            branch_rate = "—"
            if any(line.get("condition-coverage") for line in measured_lines):
                branch_rate = f"{float(coverage_class.attrib['branch-rate']):.1%}"
            summary_name = name.replace("|", "&#124;")
            summary.append(f"| {summary_name} | {line_rate} | {branch_rate} |\n")
            html.append(render_class(coverage_class, name, line_rate, branch_rate, measured_lines, sources))
        summary.append("\n")

    output_dir = results_dir / "CoverageReport"
    output_dir.mkdir(exist_ok=True)
    (output_dir / "SummaryGithub.md").write_text("".join(summary), encoding="utf-8")
    (output_dir / "index.html").write_text("".join(html) + "</main>\n</html>\n", encoding="utf-8")


if __name__ == "__main__":
    main()
