# DeveloperStore Sales API

API REST desenvolvida para gerenciamento de **Vendas** e seus respectivos **Itens de Venda**, utilizando arquitetura orientada ao domínio (DDD), EF Core, PostgreSQL e .NET 8. Este projeto foi desenvolvido como parte de um desafio técnico.

---

## O que foi concluído

- Cadastro de venda com pelo menos um item
- Adição, edição e remoção de itens da venda
- Cancelamento de venda
- Listagem geral e por ID
- Implementação da paginação, filtros e ordenação no endpoint de listagem
- Aplicação automática de regras de desconto por quantidade
- Middleware global de tratamento de exceções
- Testes unitários com xUnit, NSubstitute e Bogus
- Uso de padrões como:
  - Aggregate Root (Sale como raiz de SaleItem)
  - Value Objects (CustomerId, BranchId, ProductId, Money)
  - Specification Pattern (regras de negócio reutilizáveis)
  - Mediator (via MediatR)
  - AutoMapper para conversões entre objetos
- Persistência com PostgreSQL
- Versionamento de código com Git e uso de commits semânticos
- Correção configurações da Migration (para que gere dentro do padrão estabelecido, projeto Ambev.DeveloperEvaluation.ORM)
- Correção do enpoint de criação de usuário 
- Correção do enpoint de autenticação do usuário 
---

## Requisitos e Configuração

### Pré-requisitos

- .NET 8 SDK
- EF Core CLI Tools
- Opcional: Ferramenta de administração (ex: pgAdmin, DBeaver)

### Configurar banco de dados

1 - Criar um banco PostgreSQL 
  Opção 1: Instalar manualmente o PostgreSQL localmente e criar um banco com o nome desejado
  Opção 2: Usar Docker Compose executando o arquivo docker-compose.yml na raiz do projeto:
  ```bash
  docker-compose up -d
  ```

2. Atualize a string de conexão no `appsettings.json` do projeto `Ambev.DeveloperEvaluation.WebApi`
3. Aplique as migrations:

```bash
cd src/Ambev.DeveloperEvaluation.WebApi

dotnet ef database update --project ../Ambev.DeveloperEvaluation.ORM --startup-project ../Ambev.DeveloperEvaluation.WebApi
```


## Endpoints disponíveis (Swagger)

- `POST /api/Sales` - Criar nova venda (com ao menos 1 item)
- `GET /api/Sales` - Listar todas as vendas
- `GET /api/Sales/{id}` - Buscar venda por ID
- `PATCH /api/Sales/{id}/cancel` - Cancelar venda
- `POST /api/Sales/{saleId}/items` - Adicionar item à venda
- `PUT /api/Sales/{saleId}/items/{itemId}` - Editar item
- `DELETE /api/Sales/{saleId}/items/{itemId}` - Remover item

---

## Decisões e padrões adotados

- `Sale` é a Aggregate Root de `SaleItem`, o que significa que todas as operações sobre os itens são feitas por meio da entidade principal, garantindo consistência.
- Utilização de **Value Objects** para entidades externas (Customer, Branch, Product) conforme o padrão **External Identities com desnormalização**, otimizando leitura.
- Apenas o `SaleRepository` é necessário, pois o item não existe fora do contexto da venda.
- As validações de regras de negócio foram encapsuladas no domínio.

---

## Melhorias futuras (TODO)

- Implementar controle de permissão nos endpoints de venda, permitindo que apenas usuários com perfil Customer ou Manager possam associar vendas.
- Adicionar integração com MongoDB para logs
- Criar testes de integração e e2e
- Implementar cache com Redis
- **Publicar eventos de domínio**:
  - SaleCreated
  - SaleModified
  - SaleCancelled
  - ItemCancelled

---

## Testes com Postman

Este repositório contém uma coleção do Postman para facilitar os testes da API.
O arquivo da coleção está disponível na raiz do projeto:

`Ambev.DeveloperEvaluation.postman_collection.json`

---

## Desenvolvido por

Aline Yumi Kikuchi  
Email: aline_y.kikuchi@hotmail.com
