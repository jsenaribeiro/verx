
using FluxoCaixa.WebApi.Requests;
using FluxoCaixa.Domain.Usuarios;
using FluxoCaixa.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow;
using Shouldly;

namespace FluxoCaixa.UnitTests;

[Binding]
public class UsuarioStepDefinitions : AbstractTest
{
   private bool _sucesso;

   private List<Usuario> _usuarios;

   private IActionResult? _resultado;

   private readonly UsuarioController _controller;

   public UsuarioStepDefinitions() =>
      (_usuarios, _controller) = (new(), new UsuarioController(provider));

   [Given(@"que há somente um usuário cadastrado:")]
   public void DadoUsuarioJaCadastradoComOsDados(Table table)
   {
      unitOfWork.Usuarios.DropAsync(x => true).Wait();

      _usuarios = table.CreateSet<UsuarioDTO>()
         .Select(x => x.ToUsuario())
         .ToList();

      foreach (var usuario in _usuarios)
         unitOfWork.Usuarios.SaveAsync(usuario).Wait();
   }

   [Given(@"usuário com nome ""(.*)"", email ""(.*)"" e senha ""(.*)""")]
   public void DadoUmUsuarioComNomeEmailSenha(string nome, string email, string senha) =>
      _usuarios.Add(new Usuario(nome, email, senha));

   [Given(@"esse usuário já está registrado no sistema")]
   public void DadoEsseUsuarioJaEstaRegistradoNoSistema()
   {
      unitOfWork.Usuarios.SaveAsync(_usuarios.Last()).Wait();
      _sucesso = true;
   }

   [Given(@"esse usuário não está registrado no sistema")]
   public void DadoEsseUsuarioNaoJaEstaRegistradoNoSistema()
   {
      unitOfWork.Usuarios.DropAsync(x => x.Email == _usuarios.Last().Email).Wait();
      _sucesso = false;
   }

   [When(@"se ""(.*)"" com ""(.*)"" e ""(.*)""")]
   public void QuandoSeComE(string acao, string email, string senha)
   {
      if (acao == "cadastrar")
      {
         if (_sucesso) unitOfWork.Usuarios
            .DropAsync(x => x.Email == email).Wait();

         else DadoEsseUsuarioJaEstaRegistradoNoSistema();

         var nome = _usuarios.Last().Nome;
         var command = new SignUpCommand(nome, email, senha);
         _resultado = _controller.SignUp(command).Result;
      }

      if (acao == "autenticar" || acao == "logar")
         _resultado = _controller.SignIn(new SignInQuery(email, senha)).Result;
   }

   [Then(@"dever retornar ""(.*)""")]
   public void EntaoDeverRetornar(string mensagem) =>
      GetValueOf<string>(_resultado!)!.ShouldBe(mensagem);

   [Then(@"se deve ""(.*)"" com sucesso")]
   public void EntaoSeDeveComSucesso(string acao)
   {
      if (_resultado is not OkObjectResult valor)
         throw new Exception($"ação de '{acao}' não obteve sucesso!");
   }
}