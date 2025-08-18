namespace FluxoCaixa.UnitTests;

public class LancamentoDTO
{
   private string tipo = "";

   private string data = DateTime.Now.ToShortDateString();

   public string Tipo
   {
      get => tipo;
      set => tipo = value.Replace("'", "").Replace("\"", "");
   }

   public string Data
   {
      get => data;
      set => data = value.Replace("\"", "").Replace("'", "");
   } 

   public decimal Valor { get; set; }
}