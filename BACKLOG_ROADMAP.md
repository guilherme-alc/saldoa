# Roadmap do Saldoa

Este documento organiza o backlog em uma ordem pratica de execucao. A ideia e servir como roteiro editavel: marque os itens concluidos com `[x]`, mova tarefas entre secoes quando a prioridade mudar e acrescente notas conforme as decisoes ficarem mais claras.

## Decisao principal

Eu seguiria uma mistura das duas sugestoes:

1. Primeiro, arrumar a base que protege performance, paginacao e integridade dos dados.
2. Em paralelo, concluir pequenos itens de produto que ja estejam encaminhados.
3. Depois, fortalecer autenticacao e seguranca.
4. Em seguida, evoluir features de produto, relatorios e jobs.
5. Deixar Redis, HATEOAS, materialized views e patterns mais complexos para quando houver necessidade real.

O motivo: o Saldoa e uma aplicacao financeira. Antes de crescer em features, vale garantir que os dados sejam consistentes, que consultas comuns nao degradem rapido e que o fluxo de autenticacao esteja seguro.

## Legenda

- `[ ]` Pendente
- `[x]` Concluido
- `(em andamento)` Use no texto do item quando estiver trabalhando nele
- `(bloqueado: motivo)` Use quando depender de uma decisao ou pre-requisito

## Agora

Objetivo: deixar a base tecnica mais previsivel antes de abrir muito escopo.

### Quick win de produto

- [ ] Criar GET para obter todas as transacoes/parcelas de um `installment_group_id`.

### Paginacao

- [ ] Criar `PaginationDefaults`.
- [ ] Definir `PageNumber`, `PageSize` e `MaxPageSize`.
- [ ] Criar uma abstracao reutilizavel para filtros paginados, como `PageParameters` ou `PagedQuery`.
- [ ] Criar extensao de validacao de paginacao para validators.
- [ ] Aplicar `MaxPageSize` em todos os endpoints paginados.
- [ ] Criar testes para validacoes de paginacao.

Exemplo de referencia:

```csharp
public static class PaginationDefaults
{
    public const int PageNumber = 1;
    public const int PageSize = 20;
    public const int MaxPageSize = 100;
}
```

### Performance e integridade

- [ ] Mapear consultas principais por tabela e endpoint.
- [ ] Adicionar indices compostos nas consultas principais.
- [ ] Criar constraint no PostgreSQL para impedir budgets sobrepostos por usuario, categoria e periodo.
- [ ] Otimizar `TransactionBudgetAnalyzer` para evitar N+1 queries.
- [ ] Buscar budgets relevantes em lote pelo menor e maior periodo das parcelas.
- [ ] Buscar totais ja gastos agrupados em uma unica consulta.
- [ ] Manter alerta de budget sincrono na criacao/edicao de transacao.
- [ ] Criar testes para regras de budget e para o analyzer.

Notas:

- Nao usar Hangfire para o alerta imediato de budget.
- Nao usar Redis para compensar query ou indice ruim.
- O banco deve proteger invariantes fortes, como budget sobreposto.

### Testes essenciais

- [ ] Criar testes unitarios para use cases principais de transacao.
- [ ] Criar testes para validadores.
- [ ] Criar testes para regras de budget.
- [ ] Avaliar testes de integracao para repositorios e constraints importantes.

### Infra minima

- [ ] Usar PostgreSQL pelo Docker para desenvolvimento local.
- [ ] Documentar como subir o banco localmente.
- [ ] Garantir que migrations e connection strings funcionem no fluxo local.

## Proximo

Objetivo: fechar lacunas de seguranca e autenticacao antes de expandir features sensiveis.

### Autenticacao e seguranca

- [ ] Migrar refresh token para cookie `HttpOnly`, `Secure` e `SameSite`.
- [ ] Remover exposicao do refresh token via JavaScript.
- [ ] Avaliar mitigacao de CSRF no fluxo de refresh token.
- [ ] Implementar rate limit nos endpoints de Identity.
- [ ] Incluir pelo menos login, register, refresh, esqueci senha e confirmacao de e-mail no escopo de rate limit.
- [ ] Criar confirmacao de e-mail usando ASP.NET Core Identity.
- [ ] Criar fluxo de esqueci a senha usando ASP.NET Core Identity.
- [ ] Criar job para limpar refresh tokens expirados ou revogados antigos.
- [ ] Avaliar se o job de limpeza deve comecar como hosted service simples ou Hangfire.

### Logs

- [ ] Validar implementacao de logs com Serilog ou NLog.
- [ ] Definir eventos minimos de log para autenticacao, jobs e erros inesperados.
- [ ] Evitar logar dados financeiros sensiveis ou tokens.

## Produto

Objetivo: aumentar valor percebido do app depois que a base estiver mais firme.

### Transacoes

- [ ] Criar transacoes recorrentes.
- [ ] Validar uso de job agendado para gerar recorrencias.
- [ ] Definir regra para evitar duplicidade na geracao de recorrencias.
- [ ] Definir se recorrencias geram transacoes previstas, confirmadas ou ambas.

### Orcamentos

- [ ] Validar ajuste do limite de gasto por categoria para poder ser mensal.
- [ ] Padronizar comportamento de budgets por periodo.
- [ ] Garantir que a constraint de budgets sobrepostos continua valida para o modelo mensal.

### Alertas

- [ ] Criar alerta de compromisso/pagamento.
- [ ] Definir canais de alerta: resposta da API, notificacao interna, e-mail ou outro.
- [ ] Definir antecedencia configuravel para alertas de vencimento.

### Metas financeiras

- [ ] Criar metas financeiras.
- [ ] Criar Goals/Caixinhas.
- [ ] Definir se meta tem aporte manual, aporte recorrente ou ambos.
- [ ] Definir como metas impactam saldo disponivel.

## Relatorios e dashboards

Objetivo: entregar leituras prontas para o front sem transformar o front em montador de relatorio.

### Endpoints de relatorio

- [ ] Criar relatorio de receitas por mes.
- [ ] Criar relatorio de despesas por mes.
- [ ] Criar relatorio de saldo mensal.
- [ ] Criar relatorio de gastos por categoria.
- [ ] Criar relatorio de percentual usado do budget.
- [ ] Criar comparacao mes atual vs mes anterior.

### Views no banco

- [ ] Criar view `monthly_transaction_summary`.
- [ ] Avaliar views para gastos por categoria.
- [ ] Avaliar views para uso de budget.
- [ ] Comecar com views normais.
- [ ] Avaliar materialized views apenas se os relatorios ficarem caros.
- [ ] Usar Hangfire para refresh apenas se materialized views entrarem de fato.

Exemplo conceitual:

```sql
CREATE VIEW app.monthly_transaction_summary AS
SELECT
    user_id,
    date_trunc('month', paid_or_received_at)::date AS month,
    type,
    SUM(amount) AS total_amount,
    COUNT(*) AS transaction_count
FROM app.transactions
GROUP BY user_id, date_trunc('month', paid_or_received_at), type;
```

## Infraestrutura e DevOps

Objetivo: evoluir Docker e deploy sem travar o desenvolvimento diario.

### Docker

- [ ] Dockerizar API.
- [ ] Dockerizar banco + API com Docker Compose.
- [ ] Definir fluxo de desenvolvimento local com Docker.
- [ ] Definir como rodar migrations no ambiente Docker.
- [ ] Entender como ficaria deploy/hospedagem usando Docker.
- [ ] Avaliar se faz sentido adicionar Nginx apenas em uma etapa posterior.

### Jobs

- [ ] Introduzir Hangfire quando existir necessidade concreta.
- [ ] Usar jobs para limpeza de refresh tokens.
- [ ] Usar jobs para transacoes recorrentes.
- [ ] Usar jobs para exportacoes demoradas, se existirem.
- [ ] Usar jobs para refresh de materialized views, se existirem.

### Cache e Redis

- [ ] Validar se cache faz sentido na aplicacao.
- [ ] Nao cachear CRUD financeiro basico neste momento.
- [ ] Avaliar Redis para rate limit se a API rodar em multiplas instancias.
- [ ] Avaliar Redis para cache curto de dashboards.
- [ ] Avaliar Redis para idempotencia futura em importacao de extrato.
- [ ] Avaliar Redis para locks distribuidos apenas se houver escala/concorrencia que justifique.

## Decisoes tecnicas

Estas tarefas nao devem virar refatoracao gigante de uma vez. O ideal e decidir o padrao e aplicar incrementalmente quando tocar nos casos de uso.

### DTOs, Commands, Queries e Results

Decisao recomendada:

- [ ] Manter `Request` e `Response` na camada de API, quando o objeto representar HTTP.
- [ ] Usar `Command` para entradas de use cases que mudam estado.
- [ ] Usar `Query` para entradas de use cases de leitura.
- [ ] Usar `Result` para retornos da Application.
- [ ] Nao tratar isso como CQRS completo.
- [ ] Incluir `UserId` dentro do `Command` ou `Query` quando ele fizer parte da intencao do caso de uso.
- [ ] Migrar nomes de forma incremental, comecando por use cases novos ou alterados.

Exemplo desejado:

```csharp
var command = new CreateTransactionCommand(
    user.GetUserId(),
    request.Title,
    request.Description,
    request.PaidOrReceivedAt,
    request.Type,
    request.TotalAmount,
    request.CategoryId,
    request.TotalInstallments);

var result = await useCase.ExecuteAsync(command, ct);
```

### Mapeamentos

Decisao recomendada:

- [ ] Criar `ToDto` quando houver repeticao real de mapeamento.
- [ ] Criar `ToEntity` com cuidado, apenas quando o mapeamento nao esconder regra de negocio.
- [ ] Preferir extension methods simples ou factories explicitas.
- [ ] Evitar uma infraestrutura generica de mappers por enquanto.
- [ ] Nao introduzir AutoMapper sem dor concreta que justifique.

### Tratamento de erros

Decisao recomendada:

- [ ] Continuar usando exceptions de dominio por enquanto.
- [ ] Padronizar diferenca entre erro de validacao, erro de regra de negocio e erro inesperado.
- [ ] Avaliar Notification Pattern somente se surgir necessidade de acumular multiplos erros no mesmo fluxo.
- [ ] Nao migrar para Notification Pattern como refatoracao preventiva.

## Baixa prioridade ou estudos

Itens que podem ser interessantes para aprendizado, mas nao devem competir com produto, seguranca e integridade agora.

- [ ] Implementar HATEOAS.
- [ ] Avaliar procedures.
- [ ] Avaliar triggers para casos especificos.
- [ ] Avaliar materialized views sem necessidade comprovada.
- [ ] Avaliar Redis para locks distribuidos.
- [ ] Avaliar idempotencia para importacao de extrato.
- [ ] Avaliar Notification Pattern em profundidade.

## Backlog consolidado sem duplicacoes

- [ ] GET de transacoes por `installment_group_id`.
- [ ] `PaginationDefaults`, `MaxPageSize` e validacao reutilizavel.
- [ ] Indices compostos nas consultas principais.
- [ ] Constraint para impedir budgets sobrepostos.
- [ ] Otimizacao do `TransactionBudgetAnalyzer`.
- [ ] Testes unitarios e testes de integracao essenciais.
- [ ] Refresh token em cookie `HttpOnly`, `Secure` e `SameSite`.
- [ ] Mitigacao de CSRF no refresh token.
- [ ] Rate limit nos endpoints de Identity.
- [ ] Confirmacao de e-mail.
- [ ] Esqueci senha.
- [ ] Job de limpeza de refresh tokens.
- [ ] Logs com Serilog ou NLog.
- [ ] Transacoes recorrentes usando job agendado.
- [ ] Limite mensal de gasto por categoria.
- [ ] Alertas de compromisso/pagamento.
- [ ] Metas financeiras / Goals / Caixinhas.
- [ ] Relatorios e dashboards com queries agregadas ou views.
- [ ] Dockerizar banco e API com Docker Compose.
- [ ] Avaliar cache/Redis para casos especificos.
- [ ] HATEOAS apenas como estudo futuro.

## Sequencia sugerida por sprints

### Sprint 1 - Base curta e objetiva

- [ ] GET por `installment_group_id`.
- [ ] `PaginationDefaults`.
- [ ] Validacao reutilizavel de paginacao.
- [ ] `MaxPageSize` nos filtros paginados.
- [ ] Testes de paginacao.

### Sprint 2 - Performance e integridade

- [ ] Indices compostos.
- [ ] Constraint de budgets sobrepostos.
- [ ] Otimizacao do `TransactionBudgetAnalyzer`.
- [ ] Testes de regras de budget.
- [ ] PostgreSQL via Docker no desenvolvimento local.

### Sprint 3 - Seguranca de autenticacao

- [ ] Refresh token em cookie seguro.
- [ ] Mitigacao de CSRF.
- [ ] Rate limit no Identity.
- [ ] Confirmacao de e-mail.
- [ ] Esqueci senha.

### Sprint 4 - Jobs e observabilidade

- [ ] Job de limpeza de refresh tokens.
- [ ] Logs estruturados.
- [ ] Decidir entrada real do Hangfire.

### Sprint 5 - Relatorios iniciais

- [ ] Endpoints de resumo mensal.
- [ ] Gastos por categoria.
- [ ] Uso de budget.
- [ ] View `monthly_transaction_summary`.

### Sprint 6 - Produto avancado

- [ ] Transacoes recorrentes.
- [ ] Alertas de compromisso/pagamento.
- [ ] Metas financeiras / Caixinhas.
- [ ] Docker Compose completo com API e banco.

### Futuro

- [ ] Redis se houver motivo claro.
- [ ] Materialized views se relatorios ficarem caros.
- [ ] HATEOAS se for objetivo de estudo.
- [ ] Notification Pattern se o dominio passar a acumular muitos erros por fluxo.
