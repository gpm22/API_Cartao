# API Rest para criação de cartões aleatórios

## Descrição

O presente artigo expõe o desenvolvimento de uma API Rest que fornece um sistema de geração de número de cartão de crédito virtual criado com .Net Core e Entity Framework Core. Essa API consiste em 2 *endpoints*, onde o primeiro recebe o email do usuário e retorna um objeto contendo o email e o número do cartão criado, enquanto que o segundo *endpoint* recebe o email do usuário e retorna uma lista contendo todos os cartões de crédito em ordem de criação. Foram utilizados o sistema operacional Ubuntu 21.04 em conjunto com o editor de texto VS Code.

## 1 - Criar um projeto Web

​	O primeiro passo é criar um projeto web com o .NET Core e isso é feito através do console do ubuntu através dos seguintes passos:

1. Cria-se um pasta onde o projeto será contido.
2. Entra-se na pasta deseja e executa-se os seguintes comandos:

```bash
dotnet new webapi -o APICartao #Irá criar a aplicação
cd APICartao #Irá para a pasta da aplicação criada
dotnet add package Microsoft.EntityFrameworkCore.InMemory #Irá adicionar o framework Entity a aplicação
code -r ../APICartao #Irá abrir o programa no VS Code
```

3. Então ao aparecer uma caixa de diálogo no VS Code com a seguinte informação: "Do you want to add required assets to the project? " Selecione a opção **Yes**.

## 2- **Criar certificado de desenvolvimento HTTPS**

É necessária a criação de Certificado de desenvolvimento HTTPS (*HTTPS development certificate*) e que o mesmo seja confiado pelo sistema operacional. Isso varia de acordo com o sistema operacional e está bem explicado em [Microsoft](https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-5.0&tabs=visual-studio#ssl-linux).

## 3 - **Mudar o *launchURL***

Em `Properties\launchSettings.json` altere `launchUrl` de `swagger` para `api/APICartao`. Isso irá mudar a URL de acesso.

## 4 - **Adicionar uma classe *model***

* Uma classe ***model*** é uma classe usada para representar os dados com que o aplicativo está trabalhando;

* Nesse projeto será usado apenas uma classe chamada de `CartaoItem`.

* A criação dessa classe é feita através dos seguintes passos:

  1. Cria-se uma pasta chamada ***Models***;
  2. Cria-se uma classe chamada `CartaoItem`:

  ```c#
  namespace APICartao.Models
  {
      public class CartaoItem
      {
          public long Id {get; set;}
          public string Email {get; set;}
          public string NumeroCartao {get; set;}
      }
  }
  ```

## 5 - Criar um método para gerar o cartão de cŕedito

O número do cartão de crédito contém 16 dígitos, sendo que existe um espaçamento a cada 4 dígitos.  Cada dígito será um valor aleatório entre 0-9 e será gerado através da função **Random.Next()**. Então através de uma estrutura de repetição cada dígito será concatenado em uma string, sendo que a cada 4 dígitos um espaçamento é adicionado entre os números.

A classe `CartaoItem` com esse método implementado é mostrada a seguir:

```c#
using System;

namespace APICartao.Models
{
    public class CartaoItem
    {
        public long Id {get; set;}
        public string Email {get; set;}
        public string Cartao {get; set;}

        public void criarCartao(){
            Random rnd = new Random();

            string cartao = "";
            for (int i = 1; i < 17; i++)
            {
                cartao += rnd.Next(0, 9).ToString();

                if(i%4==0 && i<16) {
                    cartao += " ";
                }

            }

            this.Cartao = cartao;

        }

    }
}
```



## 6 - **Adicionar uma *database context***

A classe ***database context*** é a principal classe que coordenada a funcionalidade da framework **Entity** para uma **data model**. Essa classe permite a manipulação dos dados através dos objetos. Então a classe `CartaoContext` é criada na pasta ***Models*** contendo o seguinte código:

```c#
using Microsoft.EntityFrameworkCore;

namespace APICartao.Models
{
    public class CartaoContext: DbContext
    {
        public CartaoContext(DbContextOptions<CartaoContext> options) : base(options)
        {    
        }

        public DbSet<CartaoItem> CartaoItems { get; set; }

    }
}
```

## 7 - **Registre o *database context***

O **DB Context** precisa ser registrado usando o contâiner *dependency injection (DI)* que provê serviços para os controladores. Isso será feito atualizando o `Starup.cs` com o seguinte código:

```c#
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using APICartao.Models;

namespace APICartao
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CartaoContext>(opt => opt.UseInMemoryDatabase("CartaoList")); //especifica que o database context será utilizado em uma base de dados na memória
            services.AddControllers();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

```

## 8 - **Criar o controlador**

Isso é feito ao se rodar os seguintes comandos na pasta do projeto através do console:

```bash
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design #adiciona esse pacote ao aplicativo
dotnet add package Microsoft.EntityFrameworkCore.Design #adiciona esse pacote ao aplicativo
dotnet add package Microsoft.EntityFrameworkCore.SqlServer #adiciona esse pacote ao aplicativo
dotnet tool install -g dotnet-aspnet-codegenerator #Instala o scaffolding engine
dotnet aspnet-codegenerator controller -name CartaoItemsController -async -api -m CartaoItem -dc CartaoContext -outDir Controllers #Realiza a criação do CartaoItemsController atraǘes do scaffolding engine
```

Através desses comandos cria-se a classe `CartaoItemsController`, que permite ao programa responser aos *requests* do web API. Esse código usa o DI para injetar o *database context* (CartaoContext) dentro do controlador, para assim permitir que o controlador faça uso dos métodos CRUD.

## 9 - **Atualize os métodos do Controlador**

Apenas 3 métodos são utilizados nessa API:

* POST para enviar o email;
* GET para receber o número do cartão após se enviar o método POST;
* GET para receber uma lista com os cartões para um certo email;

Esses métodos estão no arquivo CartaoItemsController.cs e são mostrados a seguir:

```c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APICartao.Models;

namespace APICartao.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartaoItemsController : ControllerBase
    {
        private readonly CartaoContext _context;

        public CartaoItemsController(CartaoContext context)
        {
            _context = context;
        }

        //Método para retornar o número do cartão após o POST request
        // GET: api/CartaoItems/id/5
        [HttpGet("id/{id}")]
        public async Task<ActionResult<CartaoItem>> GetCartaoItem(long id)
        {
            var cartaoItem = await _context.CartaoItems.FindAsync(id);

            if (cartaoItem == null)
            {
                return NotFound();
            }

            return cartaoItem;
        }

        //Método para retornar a lista de cartões para um certo email
        // GET: api/CartaoItems/email/exemple@exemple.com
        [HttpGet("email/{Email}")]
        public async Task<ActionResult<IEnumerable<CartaoItem>>> GetCartaoList(String email)
        {
            // irá buscar em CartaItems todos os objetos que onde o email é igual ao fornecido como parâmetro e então irá colocar tudo isso em uma lista
            var cartaoItemList = await _context.CartaoItems.Where(s => s.Email.Equals(email)).ToListAsync();

            if (cartaoItemList == null)
            {
                return NotFound();
            }

            return cartaoItemList;
        }

		//Método para se enviar o email e criar o cartão
        // POST: api/CartaoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CartaoItem>> PostCartaoItem(CartaoItem cartaoItem)
        {
            cartaoItem.criarCartao(); //Cria o o cartão aleatório

            _context.CartaoItems.Add(cartaoItem); //Adiciona esse novo cartão
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCartaoItem), new { id = cartaoItem.Id }, cartaoItem);
        }

    }
}

```

## 10 - Exemplo do funcionamento dessa API

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

## Conclusão

Aqui foi mostrado o densenvolvimento e funcionamento de uma API para geração de números aleatórios de cartões de créditos atrelados a um email, demonstrando assim a funcionalidade e versatilidade das frameworks .NET Core e Entity Core;

## Referências

1. https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-5.0&tabs=visual-studio-code#tabpanel_2_visual-studio-code
2. https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-5.0&tabs=visual-studio#ssl-linux
