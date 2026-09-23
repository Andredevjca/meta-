using MetaMais.Interfaces.Repositories;
using System.Security.Claims;
using MetaMais.Models;
using MetaMais.Repositories;
using MetaMais.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MySqlConnector;
namespace MetaMais.Controllers;
public class ContaController(IFinanceiroRepository repositorio,Banco banco):Controller
{
 [HttpGet("/Login")] public IActionResult Login() { ViewBag.BancoDisponivel=banco.Disponivel; return View(new LoginViewModel()); }
 [HttpPost("/Login"),EnableRateLimiting("login")]
 public async Task<IActionResult> Login(LoginViewModel model)
 {
  ViewBag.BancoDisponivel=banco.Disponivel;
  if(!banco.Disponivel) ModelState.AddModelError("","Configure o MySQL e reinicie a aplicação para entrar.");
  if(!ModelState.IsValid) return View(model);
  var usuario=await repositorio.UsuarioAsync(model.Email.Trim());
  if(usuario is null || !BCrypt.Net.BCrypt.Verify(model.Senha,usuario.SenhaHash)) { ModelState.AddModelError("","E-mail ou senha inválidos."); return View(model); }
  await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier,usuario.Id.ToString()),new Claim(ClaimTypes.Name,usuario.Nome)],CookieAuthenticationDefaults.AuthenticationScheme)),new AuthenticationProperties {IsPersistent=model.LembrarMe});
  if(usuario.TrocarSenha) TempData["Aviso"]="Bem-vindo! Recomendamos alterar a senha inicial em Meu perfil.";
  return RedirectToAction("Index","Dashboard");
 }
 [HttpGet("/Cadastro")] public IActionResult Cadastro() => View(new CadastroViewModel());
 [HttpPost("/Cadastro"),EnableRateLimiting("login")]
 public async Task<IActionResult> Cadastro(CadastroViewModel model)
 {
  if(!banco.Disponivel) ModelState.AddModelError("","O banco de dados ainda não está disponível.");
  if(string.IsNullOrEmpty(model.Senha) || model.Senha.Length<8 || model.Senha.Length>72) ModelState.AddModelError("Senha","Use uma senha de 8 a 72 caracteres.");
  if(!ModelState.IsValid) return View(model);
  try { await repositorio.CriarUsuarioAsync(new Usuario {Nome=model.Nome.Trim(),Email=model.Email.Trim(),SenhaHash=BCrypt.Net.BCrypt.HashPassword(model.Senha,12)}); }
  catch(MySqlException e) when(e.Number==1062) { ModelState.AddModelError("Email","Este e-mail já está cadastrado."); return View(model); }
  TempData["Sucesso"]="Conta criada. Entre para começar seu planejamento."; return RedirectToAction(nameof(Login));
 }
 [HttpPost("/Sair"),Authorize] public async Task<IActionResult> Sair() { await HttpContext.SignOutAsync(); return RedirectToAction(nameof(Login)); }
 [HttpGet("/EsqueciSenha")] public IActionResult EsqueciSenha() => View();
 [HttpGet("/Perfil"),Authorize] public async Task<IActionResult> Perfil() => View(await repositorio.UsuarioAsync(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)));
 [HttpPost("/Perfil"),Authorize]
 public async Task<IActionResult> Perfil([Bind("Nome,Telefone,DataNascimento")] Usuario usuario,string? senhaAtual,string? novaSenha,string? confirmacaoSenha)
 {
  usuario.Id=int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
  var atual=(await repositorio.UsuarioAsync(usuario.Id))!; usuario.Email=atual.Email;
  ModelState.Remove("Email"); string? hash=null;
  if(!string.IsNullOrEmpty(novaSenha)) {
   if(novaSenha.Length<8 || novaSenha.Length>72 || novaSenha!=confirmacaoSenha) ModelState.AddModelError("","Use 8 a 72 caracteres e confirme a nova senha.");
   else if(string.IsNullOrEmpty(senhaAtual)||!BCrypt.Net.BCrypt.Verify(senhaAtual,atual.SenhaHash)) ModelState.AddModelError("","A senha atual está incorreta.");
   else hash=BCrypt.Net.BCrypt.HashPassword(novaSenha,12);
  }
  if(!ModelState.IsValid) return View(usuario);
  await repositorio.PerfilAsync(usuario,hash); TempData["Sucesso"]="Perfil atualizado."; return RedirectToAction(nameof(Perfil));
 }
}
