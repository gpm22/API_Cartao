# API Rest para criação de cartões aleatórios

## Descrição

O presente artigo expõe o desenvolvimento de uma API Rest que fornece um sistema de geração de número de cartão de crédito virtual criado com .Net Core e Entity Framework Core. Essa API consiste em 2 *endpoints*, onde o primeiro recebe o email do usuário e retorna um objeto contendo o email e o número do cartão criado, enquanto que o segundo *endpoint* recebe o email do usuário e retorna uma lista contendo todos os cartões de crédito em ordem de criação. Foram utilizados o sistema operacional Ubuntu 21.04 em conjunto com o editor de texto VS Code.

## Exemplo do funcionamento dessa API

1. Através de um POST Request para a URL https://localhost:5001/api/CartaoItems se envia um arquivo JSON contendo o email do usuário:

```json
{
    "Email":email@email.com
}
```

2. Então o seguinte JSON é retornado:

```json
{
    "id": 1,
    "email": "email@email.com",
    "cartao": "5225 3623 7352 5733"
}
```

3. Para se testar o *endpoint* da lista de emails, o passo 1 foi repetido diversas vezes alterando-se o email algumas vezes. 

4. Então um GET Request é feito para URL https://localhost:5001/api/CartaoItems/email/email@email.com retornando:

   ```json
   [
       {
           "id": 1,
           "email": "email@email.com",
           "cartao": "5225 3623 7352 5733"
       },
       {
           "id": 2,
           "email": "email@email.com",
           "cartao": "7141 3070 4186 6320"
       },
       {
           "id": 3,
           "email": "email@email.com",
           "cartao": "8345 1384 0200 2534"
       },
       {
           "id": 4,
           "email": "email@email.com",
           "cartao": "1785 5245 0416 5873"
       },
       {
           "id": 5,
           "email": "email@email.com",
           "cartao": "5655 2581 2053 4075"
       },
       {
           "id": 6,
           "email": "email@email.com",
           "cartao": "4722 1182 4438 7527"
       },
       {
           "id": 7,
           "email": "email@email.com",
           "cartao": "0215 8476 0761 6324"
       },
       {
           "id": 8,
           "email": "email@email.com",
           "cartao": "7260 5616 5710 5848"
       },
       {
           "id": 12,
           "email": "email@email.com",
           "cartao": "0855 8747 0061 2363"
       },
       {
           "id": 14,
           "email": "email@email.com",
           "cartao": "2107 8775 5787 6741"
       }
   ]
   ```

   * A mudança dos valores da id de 8 para 12 demonstra que os cartões de outros usuários não são retornados;

## 