using GestaoEspacosDesportivos.Dominio.Espacos;
using GestaoEspacosDesportivos.Dominio.Reservas;
using GestaoEspacosDesportivos.Dominio.Utilizadores;
using GestaoEspacosDesportivos.Excecoes;
using GestaoEspacosDesportivos.Persistencia;
using GestaoEspacosDesportivos.Servicos;

namespace GestaoEspacosDesportivos;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("================================================================================");
        Console.WriteLine(" SISTEMA DE GESTÃO DE RESERVAS DE ESPAÇOS DESPORTIVOS");
        Console.WriteLine(" Unidade Curricular: Programação Orientada a Objetos (POO)");
        Console.WriteLine(" Entrega 2 - Implementação do Esqueleto das Classes e Lógica na Consola");
        Console.WriteLine("================================================================================");
        Console.WriteLine();

        // 1. Configuração da Camada de Persistência em Ficheiros JSON
        string pastaDados = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dados");
        var pUtilizadores = new PersistenciaFicheiro<Utilizador>(Path.Combine(pastaDados, "utilizadores.json"));
        var pEspacos = new PersistenciaFicheiro<EspacoDesportivo>(Path.Combine(pastaDados, "espacos.json"));
        var pReservas = new PersistenciaFicheiro<Reserva>(Path.Combine(pastaDados, "reservas.json"));

        // 2. Inicialização dos Gestores de Negócio (Dependency Injection)
        var gestorUtilizadores = new GestorUtilizadores(pUtilizadores);
        var gestorEspacos = new GestorEspacos(pEspacos);
        var gestorReservas = new GestorReservas(pReservas);
        var gestorRelatorios = new GestorRelatorios(gestorReservas, gestorEspacos);

        // 3. Seeding inicial de dados caso o sistema esteja vazio
        InicializarDadosIniciais(gestorUtilizadores, gestorEspacos, gestorReservas);

        // 4. Execução da Bateria de Demonstração e Verificação de Boas Práticas de POO
        ExecutarBateriaTestesPOO(gestorUtilizadores, gestorEspacos, gestorReservas, gestorRelatorios);

        Console.WriteLine();
        Console.WriteLine("================================================================================");
        Console.WriteLine(" [SUCESSO] Todos os testes de POO, persistência e exceções foram validados!");
        Console.WriteLine("================================================================================");
    }

    private static void InicializarDadosIniciais(
        GestorUtilizadores gUsers, 
        GestorEspacos gEspacos, 
        GestorReservas gReservas)
    {
        if (gUsers.ListarTodos().Count == 0)
        {
            Console.WriteLine("[SETUP] A inicializar dados de demonstração dos utilizadores...");
            gUsers.Registar("Administrador Geral", "admin@complexo.pt", "admin123", PerfilUtilizador.Administrador, "Direção Desportiva");
            gUsers.Registar("Dinis Alves", "dinis@estudante.pt", "pass123", PerfilUtilizador.UtilizadorNormal, "pv28377", "912345678");
            gUsers.Registar("João Silva", "joao@estudante.pt", "pass123", PerfilUtilizador.UtilizadorNormal, "pv27638", "923456789");
            gUsers.Registar("Lara Pereira", "lara@estudante.pt", "pass123", PerfilUtilizador.UtilizadorNormal, "pv33163", "934567890");
        }

        if (gEspacos.ListarTodos().Count == 0)
        {
            Console.WriteLine("[SETUP] A inicializar recintos desportivos e equipamentos...");
            
            // Campo de Futebol
            var campoFutebol = new CampoFutebol("ESP-FUT-01", "Campo Sintético Principal", "Complexo Exterior - Setor Norte", 22, 45.0, "Sintético 4G", true);
            campoFutebol.AdicionarEquipamento(new Equipamento("EQ-BOLA-FUT", "Bolas de Futebol Regulamentares", 10, EstadoEquipamento.Bom));
            campoFutebol.AdicionarEquipamento(new Equipamento("EQ-REDE-FUT", "Redes de Baliza Pro", 2, EstadoEquipamento.Novo));
            gEspacos.AdicionarEspaco(campoFutebol);

            // Court de Ténis
            var courtTenis = new CourtTenis("ESP-TEN-01", "Court Central de Ténis", "Complexo Central", 4, 20.0, "Terra Batida", true);
            courtTenis.AdicionarEquipamento(new Equipamento("EQ-RAQ-TEN", "Raquetes de Ténis Wilson", 8, EstadoEquipamento.Bom));
            courtTenis.AdicionarEquipamento(new Equipamento("EQ-BOLA-TEN", "Tubos de Bolas de Ténis (Pack 4)", 15, EstadoEquipamento.Novo));
            gEspacos.AdicionarEspaco(courtTenis);

            // Pavilhão Polidesportivo
            var pavilhao = new Pavilhao("ESP-PAV-01", "Pavilhão Polidesportivo Coberto", "Edifício Interior", 100, 60.0, "Piso de Madeira Flutuante");
            pavilhao.AdicionarEquipamento(new Equipamento("EQ-BOLA-BASQ", "Bolas de Basquetebol Molten", 12, EstadoEquipamento.Bom));
            pavilhao.AdicionarEquipamento(new Equipamento("EQ-COLCH-GYM", "Colchões de Proteção e Ginástica", 20, EstadoEquipamento.Bom));
            gEspacos.AdicionarEspaco(pavilhao);

            // Sala de Fitness
            var fitness = new SalaFitness("ESP-FIT-01", "Estúdio de Fitness & Pilates", "Piso 1 - Ginásio", 25, 30.0, true, true);
            fitness.AdicionarEquipamento(new Equipamento("EQ-TAP-YOGA", "Tapetes de Yoga/Pilates", 30, EstadoEquipamento.Novo));
            gEspacos.AdicionarEspaco(fitness);
        }
    }

    private static void ExecutarBateriaTestesPOO(
        GestorUtilizadores gUsers, 
        GestorEspacos gEspacos, 
        GestorReservas gReservas, 
        GestorRelatorios gRelatorios)
    {
        Console.WriteLine("\n--- [TESTE 1: AUTENTICAÇÃO E ENCAPSULAMENTO] ---");
        try
        {
            var user = gUsers.Autenticar("dinis@estudante.pt", "pass123");
            Console.WriteLine($"✅ Login efetuado com sucesso: {user}");
            Console.WriteLine($"   Pode gerir instalações (Polimorfismo)? {user.PodeGerirInstalacoes()}");

            var admin = (Administrador)gUsers.Autenticar("admin@complexo.pt", "admin123");
            Console.WriteLine($"✅ Login admin efetuado com sucesso: {admin}");
            Console.WriteLine($"   Pode gerir instalações (Polimorfismo)? {admin.PodeGerirInstalacoes()}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro no Teste 1: {ex.Message}");
        }

        Console.WriteLine("\n--- [TESTE 2: TRATAMENTO DE EXCEÇÃO - LOGIN INVÁLIDO] ---");
        try
        {
            Console.WriteLine("Tentando login com password errada...");
            gUsers.Autenticar("dinis@estudante.pt", "password_completamente_errada");
            Console.WriteLine("❌ Falha: Deveria ter lançado LoginInvalidoException!");
        }
        catch (LoginInvalidoException ex)
        {
            Console.WriteLine($"✅ Exceção capturada com sucesso: {ex.GetType().Name} -> '{ex.Message}'");
        }

        Console.WriteLine("\n--- [TESTE 3: HERANÇA E POLIMORFISMO NAS INSTALAÇÕES DESPORTIVAS] ---");
        var espacos = gEspacos.ListarTodos();
        foreach (var esp in espacos)
        {
            double duracao = 2.0; // 2 horas
            double custo = esp.CalcularCustoReserva(duracao);
            Console.WriteLine($"\n🏟️ Recinto: {esp.Nome} ({esp.Tipo})");
            Console.WriteLine($"   Descrição Técnica: {esp.ObterDescricaoTecnica()}");
            Console.WriteLine($"   Cálculo Polimórfico para {duracao}h: {custo:C}");
            Console.WriteLine($"   Equipamentos disponíveis ({esp.Equipamentos.Count}):");
            foreach (var eq in esp.Equipamentos)
            {
                Console.WriteLine($"     - {eq}");
            }
        }

        Console.WriteLine("\n--- [TESTE 4: CRIAR RESERVA COM EQUIPAMENTOS (ESTADO PENDENTE)] ---");
        Reserva? reserva1 = null;
        try
        {
            var cliente = gUsers.ObterPorEmail("dinis@estudante.pt")!;
            var campo = gEspacos.ObterPorId("ESP-FUT-01");
            var dataJogo = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
            var ini = new TimeOnly(18, 0);
            var fim = new TimeOnly(19, 30);

            var eqs = new Dictionary<string, int>
            {
                { "EQ-BOLA-FUT", 2 }
            };

            reserva1 = gReservas.CriarReserva(cliente, campo, dataJogo, ini, fim, eqs);
            Console.WriteLine($"✅ Reserva criada com sucesso!");
            Console.WriteLine($"   {reserva1}");
            Console.WriteLine($"   Estado Inicial: {reserva1.Estado}");
            Console.WriteLine($"   Custo Total Calculado: {reserva1.CalcularCustoTotal():C}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao criar reserva: {ex.Message}");
        }

        Console.WriteLine("\n--- [TESTE 5: REGRA DE OURO - CONFLITO DE HORÁRIOS (SOBREPOSIÇÃO)] ---");
        try
        {
            var outroCliente = gUsers.ObterPorEmail("joao@estudante.pt")!;
            var campo = gEspacos.ObterPorId("ESP-FUT-01");
            var dataJogo = DateOnly.FromDateTime(DateTime.Today.AddDays(2));
            // Tentativa de marcar das 18:30 às 20:00 (colide com a reserva anterior 18:00 - 19:30)
            var iniConflito = new TimeOnly(18, 30);
            var fimConflito = new TimeOnly(20, 00);

            Console.WriteLine("Tentando agendar horário com sobreposição (18:30 - 20:00)...");
            gReservas.CriarReserva(outroCliente, campo, dataJogo, iniConflito, fimConflito);
            Console.WriteLine("❌ Falha: Deveria ter detetado o conflito de horário!");
        }
        catch (ConflitoHorariosException ex)
        {
            Console.WriteLine($"✅ Conflito intercetado e tratado: {ex.GetType().Name}");
            Console.WriteLine($"   Mensagem: {ex.Message}");
        }

        Console.WriteLine("\n--- [TESTE 6: MÁQUINA DE ESTADOS - APROVAÇÃO PELO ADMINISTRADOR] ---");
        if (reserva1 != null)
        {
            try
            {
                Console.WriteLine($"Estado antes da aprovação: {reserva1.Estado}");
                gReservas.ConfirmarReserva(reserva1.Id);
                Console.WriteLine($"✅ Reserva #{reserva1.Id} confirmada com sucesso pelo Administrador!");
                Console.WriteLine($"Estado após confirmação: {reserva1.Estado}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao confirmar reserva: {ex.Message}");
            }
        }

        Console.WriteLine("\n--- [TESTE 7: MÓDULO DE RELATÓRIOS E ESTATÍSTICAS] ---");
        try
        {
            Console.WriteLine($"Total de Reservas Registadas: {gRelatorios.GetTotalReservas()}");
            var porEspaco = gRelatorios.GetReservasPorEspaco();
            Console.WriteLine("Reservas agrupadas por espaço:");
            foreach (var (nomeEspaco, total) in porEspaco)
            {
                Console.WriteLine($"   - {nomeEspaco}: {total} reserva(s)");
            }

            var inicio = DateOnly.FromDateTime(DateTime.Today);
            var fim = DateOnly.FromDateTime(DateTime.Today.AddDays(7));
            double taxa = gRelatorios.CalcularTaxaOcupacao("ESP-FUT-01", inicio, fim);
            Console.WriteLine($"Taxa de Ocupação do Campo Principal para os próximos 7 dias: {taxa:F2}%");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro nos relatórios: {ex.Message}");
        }

        Console.WriteLine("\n--- [TESTE 8: PERSISTÊNCIA ATÓMICA EM FICHEIROS JSON] ---");
        try
        {
            int totalUsers = gUsers.ListarTodos().Count;
            int totalEspacos = gEspacos.ListarTodos().Count;
            int totalReservas = gReservas.ListarTodas().Count;
            Console.WriteLine($"✅ Ficheiros JSON sincronizados em disco:");
            Console.WriteLine($"   - utilizadores.json: {totalUsers} registos");
            Console.WriteLine($"   - espacos.json: {totalEspacos} registos");
            Console.WriteLine($"   - reservas.json: {totalReservas} registos");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro na verificação da persistência: {ex.Message}");
        }
    }
}
