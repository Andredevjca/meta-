# Meta+

Planejador financeiro por objetivos em ASP.NET Core MVC (.NET 10), Razor, Dapper e MySQL. Interface em português, Bootstrap 5, Nunito, Font Awesome e JavaScript sem jQuery.

## Executar

1. Instale o SDK .NET 10 e inicie seu servidor MySQL local na porta 3306.
2. Configure `ConnectionStrings:MySql` em `appsettings.json`. O padrão é `localhost:3306`, banco `metamais`, usuário `root`, sem senha. Para senha, prefira variável de ambiente na sessão PowerShell:

   ```powershell
   $env:ConnectionStrings__MySql = 'Server=localhost;Port=3306;Database=metamais;User ID=root;Password=SUA_SENHA;'
   ```

3. Na pasta do projeto:

   ```powershell
   dotnet restore
   dotnet run --launch-profile http
   ```

4. Abra http://localhost:5151. A rota inicial redireciona para `/Login`.

O usuário do MySQL precisa ter permissão para criar o banco e as tabelas na primeira execução. O esquema está em `Queries/Esquema.sql` e também acompanha a publicação.

## Acesso inicial

- E-mail: **admin@admin.com**
- Senha: **admin**

O administrador é criado automaticamente **quando a conexão MySQL está disponível**. A senha fica armazenada somente como hash BCrypt com custo 12. Reiniciar a aplicação não sobrescreve uma senha já alterada. Após o primeiro acesso, o sistema recomenda alterá-la em Meu perfil.

Sem MySQL, a página de login informa a configuração pendente. Não há persistência simulada nem login que contorne a autenticação.

## Funcionalidades

- Login com cookie HttpOnly, lembrar-me, cadastro, perfil e alteração de senha mediante a senha atual.
- Receitas e despesas recorrentes ou eventuais, edição, exclusão e ativação.
- Dívidas com parcelas, progresso e saldo restante.
- Objetivos com frequência, categoria, ícone, prioridade, prazo e status.
- Contribuições transacionais, histórico, conclusão automática e recálculo.
- Dashboard com capacidade financeira, objetivos, margem e próximos check-ins.
- Calendário de recebimentos, despesas, parcelas, check-ins, prazos e contribuições.
- Notificações internas e leitura de mensagens.
- Relatórios com evolução acumulada, distribuição de despesas e objetivos.
- Simulação de valor necessário e simulação inversa por contribuição mensal.
- Layout responsivo, navegação recolhível, filtros, confirmações e estados vazios.

## Regras de cálculo

O backend usa `decimal`. Vencimentos mensais são ancorados na data inicial com `AddMonths`, preservando o dia original nos meses seguintes e respeitando anos bissextos. Semanal significa sete dias e quinzenal quinze dias. Quando o prazo termina antes da próxima ocorrência, a data limite se torna o último vencimento. O dia de hoje é elegível; após uma contribuição hoje, esse vencimento é encerrado. Antecipações reduzem o saldo sem remover vencimentos futuros. Valores são arredondados para cima em centavos.

As projeções mensal, quinzenal e semanal são alternativas calculadas pelo calendário; não são uma divisão fixa por 30 dias. Prazo vencido sinaliza ajuste necessário e não produz divisão por zero. A previsão de conclusão pressupõe cumprir os próximos aportes calculados, sem rendimentos.

A capacidade do mês considera as ocorrências reais de receitas/despesas semanais e quinzenais e apenas os eventuais daquele mês. Dívidas ativas entram com uma parcela mensal, inclusive quando há parcelas vencidas; o sistema não calcula juros ou mora.

## Estrutura do projeto

- `Controllers`: um controller por tela da navegação, com rotas e respostas HTTP.
- `Controllers/Objetivos`: arquivos parciais separados para listagem, formulário, detalhes e contribuição. Mantêm o mesmo controller MVC para preservar as URLs e os formulários `/Objetivos/...`.
- `Services`: regras das telas (login, cadastro, perfil, receitas, despesas, dívidas, calendário, relatórios, notificações e simulador). `Services/Objetivos` separa as operações de cada tela dos objetivos.
- `Repositories`: persistência dividida em usuários, objetivos, contribuições, lançamentos, dívidas e notificações. Operações relacionadas continuam na mesma transação, incluindo histórico e notificações das contribuições.
- `Interfaces/Services` e `Interfaces/Repositories`: contratos das respectivas implementações.
- `Dependencias/InjecaoDependencias.cs`: registro central das dependências.
- `Models`: uma entidade por arquivo; `ViewModels/<Tela>`: dados de apresentação e validação dos formulários.
- `Views/<Tela>`: cada tela tem sua pasta, com `Index.cshtml` e, quando necessário, `Editar.cshtml`. Objetivos possui subpastas `Listagem`, `Formulario`, `Detalhes` e `Contribuicao`.
- `Views/Shared`: somente layouts, navegação e componentes compartilhados.
- Receitas e despesas têm controllers, services e views próprios e compartilham a persistência e as regras comuns de lançamentos. O planejamento financeiro é compartilhado pelas telas que usam o mesmo resumo.
- Telas informativas, como configurações e recuperação de senha, não precisam de repositories ou services sem operações próprias.
- wwwroot/js/site.js: telas e formularios carregados sem recarregar o layout, com suporte ao historico do navegador.

## Build

Execute `dotnet build`.

Para verificar rotas, renderização Razor, injeção de dependências, autorização, antiforgery e regras extraídas, execute `dotnet run --project Tests/MetaMais.Verificacoes.csproj`. As verificações usam repositórios em memória e um servidor local temporário; não inicializam nem acessam o MySQL.

## Limites desta instalação

- Recuperação de senha por e-mail requer configurar um provedor de envio; a tela informa essa indisponibilidade, sem simular envio.
- Preferências de moeda e data são BRL e dd/MM/yyyy nesta versão; não há conversão monetária.
- Não há importação bancária, movimentação real, juros, monetização ou dados de demonstração.
- Fontes e ícones vêm de CDN e precisam de acesso à internet; Bootstrap é local.
- Para publicação, use HTTPS e configure a conexão por segredo/variável de ambiente.

Não use o usuário MySQL `root` na publicação. Após inicializar, configure um usuário restrito ao banco `metamais`.
