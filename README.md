# FI.WebAtividadeEntrevista

Aplicação web para cadastro e consulta de clientes e seus beneficiários. O projeto foi desenvolvido como solução de entrevista técnica e organiza a aplicação em camadas de apresentação, negócio, acesso a dados e modelo.

## Funcionalidades

- Listagem paginada de clientes, com consulta, ordenação e alteração dos cadastros existentes.
- Cadastro obrigatório de CPF no formulário, com máscara `999.999.999-99` e validação dos dígitos verificadores.
- Impedimento de CPF duplicado entre clientes, com mensagem de validação na aplicação e índice único no banco de dados.
- Cadastro de beneficiários associado ao cliente, com CPF e nome.
- Inclusão, alteração e exclusão de beneficiários em uma janela modal.
- Impedimento de CPF repetido entre os beneficiários do mesmo cliente. O mesmo CPF pode ser usado por clientes diferentes.
- Persistência dos dados no banco LocalDB em `FI.WebAtividadeEntrevista/App_Data/BancoDeDados.mdf`.

As validações são feitas no navegador para orientar o preenchimento e também no servidor. A gravação do cliente e a sincronização dos beneficiários são executadas na mesma transação: se uma das operações falhar, as alterações do conjunto são revertidas.

## Ajustes realizados

- Corrigido o erro `O campo Id é obrigatório` que impedia salvar um novo cliente. No cadastro, o identificador não precisa ser informado previamente: o banco gera o ID e a aplicação o utiliza para associar os beneficiários ao cliente durante a mesma transação.

## Tecnologias

- C# e .NET Framework 4.8.
- ASP.NET MVC 5 e Razor.
- SQL Server LocalDB.
- jQuery 3.6.0 e jQuery Validation 1.19.5.
- Stored procedures para as operações de banco de dados.

## Organização da solução

- `FI.WebAtividadeEntrevista`: aplicação MVC, telas, validações, controlador e inicialização do banco.
- `FI.AtividadeEntrevista`: camadas de negócio (`BLL`), acesso a dados (`DAL`) e modelos de domínio (`DML`).
- `Banco/Script_Banco.sql`: script idempotente de criação e atualização do schema e das stored procedures.
- `FI.WebAtividadeEntrevista.sln`: solução do Visual Studio.

## Requisitos para executar

- Windows com Visual Studio e suporte ao desenvolvimento ASP.NET para .NET Framework.
- .NET Framework 4.8 Developer Pack.
- SQL Server Express LocalDB.

## Como executar

1. Abra `FI.WebAtividadeEntrevista.sln` no Visual Studio.
2. Restaure os pacotes NuGet da solução.
3. Defina `FI.WebAtividadeEntrevista` como projeto de inicialização.
4. Execute com `F5` ou `Ctrl+F5`.

A aplicação aponta para `App_Data/BancoDeDados.mdf` por meio da conexão `BancoDeDados` em `FI.WebAtividadeEntrevista/Web.config`. No início da aplicação, `DatabaseInitializer` cria a coluna de CPF, a tabela de beneficiários, os índices e as stored procedures que estiverem faltando. A inicialização pode ser executada novamente sem recriar objetos existentes.

Para preparar o banco manualmente, execute `Banco/Script_Banco.sql` no SQL Server que será usado pela aplicação e ajuste a conexão `BancoDeDados` em `Web.config` para esse ambiente. Não é necessário executar o script manualmente quando a aplicação tem acesso ao LocalDB configurado.

## Fluxo de verificação

1. Abra a listagem de clientes e escolha **Cadastrar Cliente**.
2. Tente salvar sem CPF, com CPF inválido e com CPF já cadastrado; cada caso deve ser rejeitado.
3. Informe um CPF válido e preencha os demais campos obrigatórios.
4. Abra **Beneficiários**, inclua um beneficiário e confira que CPF repetido para o mesmo cliente é rejeitado.
5. Altere e exclua beneficiários; salve o cliente.
6. Reabra o cliente para confirmar que seus dados e beneficiários foram persistidos.

## Evidências de teste

As evidências de teste são encaminhadas ao avaliador como anexo por e-mail e WhatsApp. O relatório reúne capturas de tela de nove cenários funcionais relacionados ao cadastro de clientes e beneficiários:

- Exibição do campo CPF e sua máscara.
- Obrigatoriedade do CPF.
- Validação dos dígitos verificadores.
- Bloqueio de CPF já cadastrado para outro cliente.
- Exibição dos campos de beneficiário.
- Inclusão de beneficiário.
- Bloqueio de CPF repetido para o mesmo cliente.
- Alteração de beneficiário.
- Exclusão de beneficiário.

O documento apresenta o resultado e as capturas de cada cenário. As evidências correspondem a verificações manuais e não representam uma suíte de testes automatizados.

## Persistência

Os dados ficam no arquivo MDF configurado em `App_Data` e não expiram ao fechar o navegador ou reiniciar a aplicação. Cada cópia ou extração do projeto tem seu próprio arquivo de banco; para consultar os mesmos registros, execute sempre a cópia que contém o MDF onde eles foram gravados.
