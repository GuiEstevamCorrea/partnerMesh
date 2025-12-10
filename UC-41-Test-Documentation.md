# Teste do UC-41 - Atualizar Tipo de Negócio

## API Endpoint
`PUT /api/business-types/{id}`

## Autenticação
Bearer Token JWT necessário com permissões AdminGlobal ou AdminVetor

## Descrição
Permite atualizar as informações de um tipo de negócio existente.

## Payload de Exemplo

```json
{
  "name": "Consultoria Empresarial Atualizada",
  "description": "Serviços de consultoria empresarial com foco em produtividade e gestão - ATUALIZADA"
}
```

## Cenários de Teste

### 1. Atualização com Sucesso
- **Pré-requisito**: Ter um tipo de negócio existente e token JWT válido
- **Input**: Dados válidos para atualização
- **Output Esperado**: Status 200 OK com dados atualizados

### 2. Tipo de Negócio Não Encontrado
- **Input**: ID inexistente
- **Output Esperado**: Status 404 Not Found

### 3. Nome Já Existente
- **Input**: Nome que já existe em outro tipo de negócio
- **Output Esperado**: Status 400 Bad Request

### 4. Usuário Sem Permissão
- **Input**: Token de usuário sem permissão AdminGlobal ou AdminVetor
- **Output Esperado**: Status 403 Forbidden

### 5. Campos Obrigatórios Vazios
- **Input**: Nome ou descrição vazios/nulos
- **Output Esperado**: Status 400 Bad Request

## Fluxo de Teste Completo

1. **Autenticação**
   ```json
   POST /api/auth/login
   {
     "email": "admin@sistema.com",
     "password": "Senha123!"
   }
   ```

2. **Criar Tipo de Negócio** (se não existir)
   ```json
   POST /api/business-types
   {
     "name": "Consultoria Empresarial",
     "description": "Serviços de consultoria empresarial"
   }
   ```

3. **Atualizar Tipo de Negócio**
   ```json
   PUT /api/business-types/{id}
   {
     "name": "Consultoria Empresarial Atualizada",
     "description": "Serviços de consultoria empresarial com foco em produtividade e gestão - ATUALIZADA"
   }
   ```

4. **Verificar Atualização**
   ```json
   GET /api/business-types/{id}
   ```

## Validações Implementadas

1. **Autenticação**: Verificação de token JWT válido
2. **Autorização**: Permissões AdminGlobal ou AdminVetor
3. **Existência**: Verificação se o tipo de negócio existe
4. **Unicidade**: Nome não pode duplicar outro tipo existente
5. **Campos Obrigatórios**: Nome e descrição devem ser preenchidos
6. **Auditoria**: Registro de quem e quando modificou

## Resposta de Sucesso

```json
{
  "id": "guid-do-tipo-negocio",
  "name": "Consultoria Empresarial Atualizada",
  "description": "Serviços de consultoria empresarial com foco em produtividade e gestão - ATUALIZADA",
  "active": true,
  "createdAt": "2024-12-10T10:00:00Z",
  "lastModified": "2024-12-10T15:30:00Z",
  "createdBy": "guid-usuario-criador",
  "modifiedBy": "guid-usuario-modificador"
}
```

## Estrutura da Solução

- **DTO**: `UpdateBusinessTypeRequest` e `UpdateBusinessTypeResult`
- **Interface**: `IUpdateBusinessTypeUseCase`
- **Use Case**: `UpdateBusinessTypeUseCase`
- **Repository**: Utiliza métodos existentes no `IBusinessTypeRepository`
- **Controller**: Endpoint PUT no `BusinessTypesController`

## Observações

- O endpoint mantém compatibilidade com a arquitetura hexagonal
- Implementa todas as validações de negócio necessárias
- Segue os padrões de autenticação e autorização estabelecidos
- Inclui tratamento de exceções adequado
- Documentação completa no Swagger/OpenAPI