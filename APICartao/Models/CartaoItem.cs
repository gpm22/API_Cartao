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