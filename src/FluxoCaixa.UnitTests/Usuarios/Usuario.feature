# language: pt-BR

Funcionalidade: Autenticação de Usuários
  Como um usuário do sistema
  Quero realizar SignIn e SignUp
  Para criar e acessar minha sessão

Contexto:
   Dado que há somente um usuário cadastrado:
   | nome    | email             | senha |
   | "teste" | "teste@email.com" | "123" |

Esquema do Cenário: Sucessos
   Dado usuário com nome <usuario>, email <email> e senha <senha>
   E esse usuário já está registrado no sistema
   Quando se <acao> com <email> e <senha>
   Então se deve <acao> com sucesso

Exemplos:
   | acao        | usuario    | senha | email                |
   | "logar"     | "teste"    | "123" | "teste@email.com"    |
   | "cadastrar" | "beltrano" | "123" | "beltrano@email.com" |

Esquema do Cenário: Exceções
   Dado usuário com nome <usuario>, email <email> e senha <senha>
   E esse usuário não está registrado no sistema
   Quando se <acao> com <email> e <senha>
   Então dever retornar <mensagem>   

Exemplos:
   | acao         | usuario    | senha | email             | mensagem                |
   | "autenticar" | "teste"    | "123" | "outro@email.com" | "Credenciais inválidas" |
   | "cadastrar"  | "beltrano" | "123" | "teste@email.com" | "E-mail já cadastrado"  |