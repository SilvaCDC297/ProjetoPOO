# Elemento E5 (16%) - Esqueleto das Classes e Lógica do Sistema
## Sistema de Gestão de Reservas de Espaços Desportivos
**Unidade Curricular:** Programação Orientada a Objetos (POO)  
**Entrega 2:** Implementação das Classes (Data-limite: 5 de novembro de 2026 às 23:59)  
**Linguagem:** C# (.NET 10 / .NET Core)  
**Elementos do Grupo:**
- **Dinis Alves** (pv28377)
- **João Silva** (pv27638)
- **Lara Pereira** (pv33163)

---

## 1. Visão Geral da Implementação de POO

O código-fonte desenvolvido nesta pasta `GestaoEspacosDesportivos` implementa rigorosamente o modelo concebido no **Diagrama de Classes UML** da Entrega 1, cumprindo todos os pilares essenciais da Programação Orientada a Objetos:

### 1.1 Abstração e Encapsulamento
- **Entidades de Domínio:** Todas as propriedades possuem encapsulamento com métodos de acesso controlados.
- **Invariantes de Negócio:** Construtores e métodos validam regras estritas (capacidade > 0, datas no futuro, hora de fim superior à de início, proibição de logins em branco).

### 1.2 Herança
- **Hierarquia de Utilizadores:**
  - `Utilizador` (classe abstrata base);
  - `UtilizadorNormal` (acrescenta número de sócio e telefone);
  - `Administrador` (acrescenta departamento, nível de acesso e capacidade de aprovação de reservas).
- **Hierarquia de Instalações Desportivas:**
  - `EspacoDesportivo` (classe abstrata base);
  - Subclasses concretas com atributos e regras especializadas:
    - `CampoFutebol` (piso sintético e iluminação);
    - `CourtTenis` (tipo de superfície e cobertura);
    - `Pavilhao` (piso polidesportivo flutuante e modalidades);
    - `SalaFitness` (climatização e espelhos integrais).

### 1.3 Polimorfismo
- Métodos abstratos implementados especificamente em cada subclasse de `EspacoDesportivo`:
  - `CalcularCustoReserva(double duracaoHoras)`: aplica regras de cálculo diferenciadas (taxas de iluminação noturna no futebol, custos de cobertura no ténis, manutenção no pavilhão);
  - `ObterDescricaoTecnica()`: ficha técnica detalhada por tipo de modalidade.
- Polimorfismo em `Utilizador.PodeGerirInstalacoes()`, respondendo dinamicamente em tempo de execução conforme o perfil concreto.

### 1.4 Máquina de Estados e Prevenção de Conflitos
- **Ciclo de Vida da Reserva:** Estados `Pendente`, `Confirmada`, `Cancelada`, `Concluida`, `Expirada`.
- **Regra de Não Sobreposição Temporal:** Implementada no método `Reserva.TemSobreposicao(data, inicio, fim)` através do algoritmo de interseção de intervalos `(ini < fimRes) && (fim > iniRes)`.
- Se for detetada sobreposição para o mesmo recinto, o sistema lança a exceção tipada `ConflitoHorariosException`.

### 1.5 Tratamento de Exceções de Negócio
Hierarquia completa sob a classe base `SistemaReservasException`:
- `ConflitoHorariosException`
- `LoginInvalidoException`
- `EspacoInexistenteException`
- `DadosInvalidosException`
- `TentativaReservaSemAutenticacaoException`
- `PersistenciaException`
- `EstadoReservaInvalidoException`
- `OperacaoInvalidaException`

### 1.6 Camada de Persistência em Ficheiros
- Interface genérica `IPersistencia<T>` com implementação concreta `PersistenciaFicheiro<T>`;
- Armazenamento em ficheiros JSON formatados com escrita atómica (utilizadores.json, espacos.json, reservas.json);
- Suporte nativo a tipos polimórficos (`$tipo`).

---

## 2. Como Executar e Testar a Aplicação na Consola

A partir da linha de comandos (dentro da pasta `GestaoEspacosDesportivos`):

```bash
# Compilar o projeto
dotnet build

# Executar a aplicação e bateria de testes de POO
dotnet run
```

Ao executar, o programa corre uma bateria automatizada que valida:
1. Autenticação e hash de segurança;
2. Captura de `LoginInvalidoException` com password errada;
3. Demonstração de Herança e Polimorfismo dos recintos desportivos;
4. Criação de reserva com equipamentos no estado `Pendente`;
5. Verificação de conflitos de horários com captura de `ConflitoHorariosException`;
6. Confirmação da reserva pelo Administrador (`Confirmada`);
7. Geração de relatórios e cálculo de taxas de ocupação;
8. Persistência atómica e integridade dos ficheiros JSON.
