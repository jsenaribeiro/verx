# language: pt-BR

Funcionalidade: Controle de Fluxo de Caixa
  Como usuário do fluxo de caixa 
  Quero lançar débitos e créditos
  Para gerar relatórios de saldo diário

Contexto:
   Dado usuário "teste" está logado

Esquema do Cenário: Lançamentos
  Dado que o valor de lançamento é <valor>
  E o tipo do lançamento é <tipo>
  Quando lançar um <tipo> de R$ <valor> em <data>
  Então o valor registrado será <resultado>
  E o tipo registrado será <tipo>  
  E usuário registrado é "teste"
  E a data será <data>

Exemplos: 
  | valor  | tipo      | data         | resultado |
  | 150,00 | "crédito" | "2025-08-14" | 150,00    |
  | 200,00 | "débito"  | "2025-08-14" | -200,00   |

Esquema do Cenário: Valores inválidos
   Quando lançar um <transacao> de R$ <valor> em "2025-08-14"
   Então será exibida a mensagem de erro <erro>

Exemplos:
   | transacao | valor  | erro                           |
   | "crédito" | 0,00   | "Valor não pode ser zero."     |
   | "débito"  | 0,00   | "Valor não pode ser zero."     |
   | "crédito" | -1,00  | "Valor não pode ser negativo." |
   | "débito"  | -1,00  | "Valor não pode ser negativo." |

Cenário: Gerar relatório de saldo diário
  Dado que existem os seguintes lançamentos:
    | Tipo      | Valor  |   Data       |
    | "crédito" | 150,00 | "2025-08-14" |
    | "débito"  | 50,00  | "2025-08-13" |
    | "débito"  | 50,00  | "2025-08-14" |
    | "débito"  | 50,00  | "2025-08-15" |
  Quando solicitar o saldo consolidado do dia "2025-08-14"
  Então o saldo consolidado deve ser R$ 100,00