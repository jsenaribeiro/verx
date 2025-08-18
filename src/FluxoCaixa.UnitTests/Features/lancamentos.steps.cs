using Microsoft.Extensions.DependencyInjection;
using FluxoCaixa.Domain.Lancamentos;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow;
using Shouldly;

namespace FluxoCaixa.UnitTests;

[Binding]
public class StepDefinitions : AbstractTest
{
   private LancamentoHandler _handler;

   private List<DateOnly> _datasSalvas = new();

   private (decimal saldo, DateOnly data, string erro) _resultado;

   private (decimal valor, DateOnly data) _esperado;

   public StepDefinitions()
   {
      Compose();

      _handler = provider.GetRequiredService<LancamentoHandler>();
      _esperado = (0, DateOnly.MinValue);
      _resultado = (0, DateOnly.MinValue, "");
   }

   [Given(@"que o valor de lançamento é (.*)")]
   public void DadoQueOValorDeLancamentoE_(decimal valor) => _esperado.valor = valor;

   [Given(@"que existem os seguintes lançamentos:")]
   public async Task DadoQueExistemOsSeguintesLancamentos(Table table)
   {
      var lancamentos = table.CreateSet<LancamentoDTO>();

      foreach (var dto in lancamentos)
      {
         var data = DateOnly.Parse(dto.Data);
         var valor = dto.Tipo == "crédito" ? dto.Valor : dto.Valor * -1;
         var lancamento = new Lancamento(valor) { Data = data };

         await unitOfWork.Lancamentos.SaveAsync(lancamento);

         _datasSalvas.Add(data);
      }
   }

   [Given(@"o tipo do lançamento é ""(.*)""")]
   public void DadoOTipoDoLancamentoE(string tipo)
   {
      if (tipo == "débito" && _esperado.valor > 0)
         _esperado.valor *= -1;
   }

   [When(@"eu lançar o valor na data ""(.*)""")]
   public async Task QuandoEuLancarOValorNaData(string data)
   {
      try
      {
         var dataObject = DateOnly.FromDateTime(DateTime.Parse(data));
         var tipo = _esperado.valor > 0 ? "crédito" : "débito";
         var valor = _esperado.valor;

         _esperado.data = DateOnly.Parse(data);

         if (tipo == "crédito")
         {
            var command = new CreditarCommand(valor) with { Data = dataObject };
            await _handler.HandleAsync(command);
         }

         else if (tipo == "débito")
         {
            var command = new DebitarCommand(valor * -1) with { Data = dataObject };
            await _handler.HandleAsync(command);
         }
      }
      catch (Exception ex)
      {
         _resultado.erro = ex.Message;
      }
   }

   [When(@"solicitar o saldo consolidado do dia ""(.*)""")]
   public async Task QuandoSolicitarOSaldoConsolidadoDoDia(string data)
   {
      try
      {
         var query = new SaldoDiarioQuery(data);

         _resultado.saldo = await _handler.HandleAsync(query);
      }
      catch (Exception ex)
      {
         _resultado.erro = ex.Message;
      }
   }

   [When(@"eu lançar um ""(.*)"" de R\$ (.*) na data ""(.*)""")]
   public async Task QuandoEuLancarUmDeRNaData(string tipo, decimal valor, string data)
   {
      try
      {
         var dataObject = DateOnly.FromDateTime(DateTime.Parse(data));

         if (tipo == "crédito")
         {
            var command = new CreditarCommand(valor) with { Data = dataObject };
            await _handler.HandleAsync(command);
         }

         else if (tipo == "débito")
         {
            var command = new DebitarCommand(valor) with { Data = dataObject };
            await _handler.HandleAsync(command);
         }
      }
      catch (Exception ex)
      {
         _resultado.erro = ex.Message;
      }
   }

   [Then(@"o valor registrado será (.*)")]
   public async Task EntaoOValorRegistradoSera_(decimal valor) =>
      (await CarregarLancamento(_esperado.data))?.Valor.ShouldBe(valor);

   [Then(@"o tipo registrado será ""(.*)""")]
   public async Task EntaoOTipoRegistradoSera(string tipo)
   {
      var lancamento = await CarregarLancamento(_esperado.data);

      if (tipo == "crédito") lancamento?.Valor.ShouldBePositive();
      if (tipo == "débito") lancamento?.Valor.ShouldBeNegative();
   }

   [Then(@"a data será ""(.*)""")]
   public async Task EntaoADataSera(string data)
   {
      var lancamento = await CarregarLancamento(_esperado.data);
      lancamento.Data.ShouldBe(DateOnly.Parse(data));
   }

   [Then(@"o saldo consolidado deve ser R\$ (.*)")]
   public void EntaoOSaldoConsolidadoDeveSerR_(decimal saldo) =>
      _resultado.saldo.ShouldBe(saldo);

   [Then(@"será exibida a mensagem de erro ""(.*)""")]
   public void EntaoSeraExibidaAMensagemDeErro(string mensagem) =>
      _resultado.erro?.ShouldBe(mensagem);

   private async Task<Lancamento> CarregarLancamento(DateOnly data)
   {
      var lancamento = await unitOfWork.Lancamentos
         .LoadAsync(x => x.Data == data);

      lancamento.ShouldNotBeNull();

      return lancamento!;
   }

   [AfterScenario]
   public async Task Clear()
   {
      await unitOfWork.Usuarios.DropAsync(x => x.Nome == "teste");
      await unitOfWork.Lancamentos.DropAsync(x => x.Data == _esperado.data);

      foreach (var data in _datasSalvas)
         await unitOfWork.Lancamentos
            .DropAsync(x => x.Data == data);

      _datasSalvas = new();
   }
}