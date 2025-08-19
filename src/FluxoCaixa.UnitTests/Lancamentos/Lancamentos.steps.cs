using Microsoft.Extensions.DependencyInjection;
using FluxoCaixa.Domain.Lancamentos;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow;
using Shouldly;
using FluxoCaixa.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using FluxoCaixa.Infrastructure;

namespace FluxoCaixa.UnitTests;

[Binding]
public class LancamentoStepDefinitions : AbstractTest
{
   private IActionResult? _resultado;
   private LancamentoHandler _handler;
   private LancamentoController _controller;
   private List<DateOnly> _datasSalvas = new();
   private (decimal valor, DateOnly data) _esperado;

   public LancamentoStepDefinitions()
   {
      Compose();

      _handler = provider.GetRequiredService<LancamentoHandler>();
      _esperado = (0, DateOnly.MinValue);
      _controller = new LancamentoController(provider);
   }

   [Given(@"que o valor de lançamento é (.*)")]
   public void DadoQueOValorDeLancamentoE_(decimal valor) => _esperado.valor = valor;

   [Given(@"que existem os seguintes lançamentos:")]
   public void DadoQueExistemOsSeguintesLancamentos(Table table)
   {
      var lancamentos = table.CreateSet<LancamentoDTO>();

      foreach (var dto in lancamentos)
      {
         var data = DateOnly.Parse(dto.Data);
         var valor = dto.Tipo == "crédito" ? dto.Valor : dto.Valor * -1;
         var lancamento = new Lancamento(valor) { Data = data };

         unitOfWork.Lancamentos.SaveAsync(lancamento).Wait();

         _datasSalvas.Add(data);
      }
   }

   [Given(@"o tipo do lançamento é ""(.*)""")]
   public void DadoOTipoDoLancamentoE(string tipo) =>
      _esperado.valor = tipo == "débito" && _esperado.valor > 0
         ? _esperado.valor * -1 : _esperado.valor;

   [When(@"solicitar o saldo consolidado do dia ""(.*)""")]
   public void QuandoSolicitarOSaldoConsolidadoDoDia(string data) =>
      _resultado = _controller.Get(data).Result;

   [When(@"eu lançar o valor na data ""(.*)""")]
   public void QuandoEuLancarOValorNaData(string data)
   {
      var tipo = _esperado.valor > 0 ? "crédito" : "débito";
      SendCommand(tipo, _esperado.valor, data);
   }

   [When(@"eu lançar um ""(.*)"" de R\$ (.*) na data ""(.*)""")]
   public void QuandoEuLancarUmDeRNaData(string tipo, decimal valor, string data) =>
      SendCommand(tipo, valor, data);

   [Then(@"o valor registrado será (.*)")]
   public void EntaoOValorRegistradoSera_(decimal valor) =>
      CarregarLancamento(_esperado.data)?.Valor.ShouldBe(valor);

   [Then(@"o tipo registrado será ""(.*)""")]
   public void EntaoOTipoRegistradoSera(string tipo)
   {
      var lancamento = CarregarLancamento(_esperado.data);

      if (tipo == "crédito") lancamento?.Valor.ShouldBePositive();
      if (tipo == "débito") lancamento?.Valor.ShouldBeNegative();
   }

   [Then(@"a data será ""(.*)""")]
   public void EntaoADataSera(string data) =>
      CarregarLancamento(_esperado.data)
         .Data.ShouldBe(DateOnly.Parse(data));

   [Then(@"o saldo consolidado deve ser R\$ (.*)")]
   public void EntaoOSaldoConsolidadoDeveSerR_(decimal saldo) =>
      GetValueOf<decimal>(_resultado!).ShouldBe(saldo);

   [Then(@"será exibida a mensagem de erro ""(.*)""")]
   public void EntaoSeraExibidaAMensagemDeErro(string mensagem) =>
      GetValueOf<Exception>(_resultado!)?.Message.ShouldBe(mensagem);

   private Lancamento CarregarLancamento(DateOnly data) =>
      unitOfWork.Lancamentos
         .LoadAsync(x => x.Data == data)
         .Result.ShouldNotBeNull();

   [BeforeScenario]
   public void Clear()
   {
      // unitOfWork.Usuarios.DropAsync(x => x.Nome == "teste").Wait();
      // unitOfWork.Lancamentos.DropAsync(x => x.Data == _esperado.data).Wait();

      // foreach (var data in _datasSalvas)
      //    unitOfWork.Lancamentos
      //       .DropAsync(x => x.Data == data);

      unitOfWork.Usuarios.DropAsync(x => true).Wait();
      unitOfWork.Lancamentos.DropAsync(x => true).Wait();

      _datasSalvas = new();
   }

   private void SendCommand(string tipo, decimal valor, string data)
   {
      _esperado.data = DateOnly.FromDateTime(DateTime.Parse(data));

      dynamic command = tipo == "crédito"
         ? new CreditarCommand(valor) with { Data = _esperado.data }
         : new DebitarCommand(valor * -1) with { Data = _esperado.data };

      _resultado = tipo == "crédito"
         ? _controller.Post(command).Result
         : _controller.Delete(command).Result;
   }
}