using CatalogoCursos.Api.Dominio;
using Microsoft.EntityFrameworkCore;

namespace CatalogoCursos.Api.Dados;

/// <summary>
/// Catálogo fictício, semeado pela migration (HasData): o script SQL gerado por
/// <c>dotnet ef migrations script</c> sai com schema e dados, e o banco nasce
/// igual em qualquer máquina. Ids fixos e sequenciais — HasData exige.
/// Os seis primeiros cursos (nome, categoria, resumo) são os do Figma; os demais
/// completam as categorias de Linguagens e Sustentabilidade, que o pacote de
/// ícones prevê. Os dois de inglês e normas já começaram (EAD com inscrição
/// contínua): servem de "cursando" para o aluno de demonstração.
/// </summary>
public static class DadosIniciais
{
    private const string Img = "/assets/imagens/";

    private static readonly Categoria[] Categorias =
    [
        new() { Id = 1, Slug = "matematica", Nome = "Matemática", Icone = "icon-matematica-20px.svg" },
        new() { Id = 2, Slug = "linguagens", Nome = "Linguagens", Icone = "icon-linguagens-20px.svg" },
        new() { Id = 3, Slug = "ciencias-da-natureza", Nome = "Ciências da Natureza", Icone = "icon-ciencias-natureza-20px.svg" },
        new() { Id = 4, Slug = "sustentabilidade", Nome = "Sustentabilidade", Icone = "icon-sustentabilidade-20px.svg" },
    ];

    /// <summary>Conteúdo: parágrafos separados por linha em branco; linha com "## " é subtítulo.</summary>
    private sealed record AulaSpec(string Titulo, string Descricao, string Conteudo);
    private sealed record ModuloSpec(string Titulo, AulaSpec[] Aulas);
    private sealed record CursoSpec(
        string Slug, string NomeCompleto, string NomeBreve, string Resumo, string Descricao, int CategoriaId,
        Modalidade Modalidade, int CargaHoraria, string PublicoAlvo, StatusCurso Status,
        string Imagem, DateOnly? Disponibilizacao, ModuloSpec[] Modulos);

    private static readonly CursoSpec[] Cursos =
    [
        new("matematica-precisao-medidas-escalas-metalurgia",
            "Matemática para a Precisão: Medidas e Escalas na Metalurgia", "Medidas e Escalas",
            "Aplicação de grandezas, proporções e escalas em processos de corte, furação e usinagem.",
            "Explore nosso curso de Matemática para a Precisão: Medidas e Escalas na Metalurgia, perfeito para quem deseja se destacar nesse campo! Este programa abrangente foi desenvolvido para capacitar profissionais com as habilidades e conhecimentos fundamentais para garantir medições precisas e eficientes no ambiente metalúrgico.",
            1, Modalidade.Hibrido, 40, "Operadores, ajustadores e técnicos de usinagem com ensino médio em andamento ou concluído.",
            StatusCurso.InscricoesAbertas, "banner-ava.webp", new DateOnly(2027, 1, 5),
            [
                new("Introdução", [
                    new("Introdução", "Como o curso funciona e o que você vai medir até o fim dele.",
                        "## Boas-vindas\n\nNeste curso, a matemática sai do caderno e vai para a bancada: cada conceito aparece ligado a uma peça, um instrumento ou um desenho técnico da metalurgia.\n\nAo final, você vai ler cotas, converter unidades, escolher o instrumento certo e interpretar tolerâncias com segurança."),
                    new("Boas práticas", "Dicas para aproveitar melhor o estudo online.",
                        "## Boas práticas\n\nSe você está interessado em se aprofundar em Matemática para a Precisão, especialmente em Medidas e Escalas na Metalurgia, aqui estão algumas dicas para maximizar sua experiência online. Primeiro, crie um espaço de estudo livre de distrações, onde você possa se concentrar totalmente no material.\n\nEstabeleça uma rotina de estudos, reservando horários específicos para assistir às aulas e realizar as atividades. Além disso, interaja com seus colegas e instrutores nos fóruns e chats disponíveis, pois isso enriquece o aprendizado e ajuda a esclarecer dúvidas. Não se esqueça de revisar o conteúdo regularmente e aplicar os conhecimentos em situações práticas sempre que possível.\n\nPor fim, mantenha-se atualizado sobre as novas técnicas e normas em medidas e escalas na metalurgia, já que essa área está sempre evoluindo. Com dedicação e as estratégias corretas, você estará preparado para se destacar no setor!\n\n## Recomendações\n\nSe você está interessado em se aprofundar na matemática aplicada à metalurgia, aqui estão algumas dicas valiosas. Primeiro, escolha um curso que ofereça uma abordagem prática, com aulas que incluam exercícios e estudos de caso. Isso ajudará a entender como aplicar medidas e escalas na prática. Além disso, verifique se o curso é reconhecido por instituições relevantes, pois isso pode impactar sua formação e carreira.\n\nOutro aspecto importante é a experiência dos instrutores. Procure cursos que tenham profissionais com experiência no setor, pois eles podem compartilhar insights valiosos que vão além da teoria.\n\nConsidere também a carga horária e a flexibilidade do curso. Se você trabalha, um curso que ofereça aulas noturnas ou aos finais de semana pode ser mais conveniente.\n\nPor fim, não se esqueça de se conectar com outros alunos e profissionais da área. Isso pode abrir portas e proporcionar um aprendizado ainda mais enriquecedor. Investir em um bom curso de matemática para a metalurgia é essencial para garantir precisão e eficiência no trabalho."),
                    new("Extras", "Materiais de apoio: tabela de conversões e glossário.",
                        "## Materiais de apoio\n\nGuarde a tabela de conversão entre milímetro, polegada e micrômetro: ela vai ser usada em quase todas as aulas.\n\nO glossário reúne os termos de metrologia que aparecem no curso, como resolução, exatidão, incerteza e tolerância."),
                ]),
                new("Grandezas e unidades", [
                    new("O Sistema Internacional na oficina", "Metro, quilograma e segundo no dia a dia da produção.",
                        "O Sistema Internacional padroniza as unidades para que uma peça medida em São Paulo tenha a mesma medida em qualquer lugar.\n\nNa metalurgia, o milímetro é a unidade de trabalho: desenhos, instrumentos e tolerâncias usam essa escala."),
                    new("Conversão de unidades", "Milímetro, polegada e micrômetro sem erro de vírgula.",
                        "Uma polegada vale 25,4 mm. Converter é multiplicar ou dividir por esse fator, e o erro mais comum é a vírgula no lugar errado.\n\n## Exercício\n\nConverta 3/8\" para milímetros e confira o resultado com o paquímetro na aula seguinte."),
                ]),
                new("Instrumentos de medição", [
                    new("Paquímetro", "Leitura do nônio e cuidados de medição.",
                        "O paquímetro mede dimensões externas, internas e de profundidade com resolução de 0,05 mm ou 0,02 mm.\n\nA leitura soma a escala principal com o traço do nônio que coincide; apoiar a peça corretamente evita erro de paralaxe."),
                    new("Micrômetro", "Quando a precisão precisa ir além do paquímetro.",
                        "O micrômetro chega a 0,01 mm e é o instrumento de quem controla ajuste de eixo e furo.\n\nA catraca garante a mesma pressão de medição: apertar o tambor com a mão muda o resultado."),
                ]),
                new("Escalas e tolerâncias", [
                    new("Escalas no desenho técnico", "Ampliação, redução e leitura de cotas.",
                        "Escala 1:2 significa que o desenho tem metade do tamanho real; 2:1, o dobro. A cota sempre informa a medida real.\n\nMedir o desenho com régua é um erro clássico: a escala serve para a proporção, não para a fabricação."),
                    new("Tolerâncias e ajustes", "Quanto a peça pode variar e continuar boa.",
                        "Tolerância é a variação permitida em torno da medida nominal. Um eixo de 20 mm com tolerância de ±0,05 mm é bom entre 19,95 e 20,05 mm.\n\nAjuste é a combinação de tolerâncias de eixo e furo: com folga, incerto ou com interferência."),
                ]),
            ]),
        new("geometria-modelagem-design-textil",
            "Geometria e Modelagem no Design Têxtil", "Geometria Têxtil",
            "Explora formas geométricas, simetria e padrões matemáticos presentes em tecidos e estampas.",
            "Neste curso, a geometria vira ferramenta de criação: simetrias, rotações e malhas explicam estampas, e a modelagem plana mostra como o tecido se transforma em peça com o menor desperdício possível.",
            1, Modalidade.Ead, 30, "Estudantes e profissionais de confecção, estamparia e design de moda.",
            StatusCurso.EmBreve, "hero-image.webp", new DateOnly(2027, 2, 1),
            [
                new("Formas no tecido", [
                    new("Simetria e padrões", "Eixos de simetria e repetição em estampas.",
                        "Toda estampa corrida é um padrão que se repete: a unidade mínima, chamada rapport, é desenhada uma vez e replicada.\n\nIdentificar os eixos de simetria ajuda a prever como o desenho vai se comportar na peça pronta."),
                    new("Transformações geométricas", "Translação, rotação e reflexão no desenho de estampas.",
                        "Translação desloca, rotação gira e reflexão espelha. Combinadas, essas três operações geram quase todos os padrões têxteis.\n\n## Prática\n\nMonte um padrão a partir de um único motivo usando só rotações de 90 graus."),
                ]),
                new("Modelagem plana", [
                    new("Do molde à peça", "Como medidas do corpo viram figuras planas.",
                        "O molde é uma figura plana que, costurada, cria volume. Retângulos, trapézios e curvas são calculados a partir das medidas do corpo.\n\nA folga de costura é somada ao contorno, e esquecê-la muda o tamanho final da peça."),
                    new("Encaixe e aproveitamento", "Posicionar moldes para desperdiçar menos tecido.",
                        "Encaixe é a arrumação dos moldes sobre o tecido. A área aproveitada dividida pela área total dá o rendimento do corte.\n\nPequenas rotações dos moldes, respeitando o fio do tecido, podem economizar metros em uma produção."),
                ]),
            ]),
        new("estatistica-controle-qualidade-producao",
            "Estatística e Controle de Qualidade na Produção", "Estatística na Qualidade",
            "Leitura e interpretação de dados de controle de qualidade e produtividade.",
            "Amostragem, média, desvio-padrão e cartas de controle para acompanhar a estabilidade de um processo e decidir com base em dados, não em impressão.",
            1, Modalidade.Hibrido, 60, "Inspetores de qualidade, líderes de produção e técnicos de processo.",
            StatusCurso.EmBreve, "video-thumb.webp", new DateOnly(2026, 11, 16),
            [
                new("Fundamentos de estatística", [
                    new("Amostra e população", "Como escolher uma amostra representativa do lote.",
                        "Medir todas as peças de um lote quase nunca é viável. A amostra precisa representar o lote: aleatória e do tamanho certo.\n\nAmostra tirada só do começo do turno esconde o desgaste da ferramenta no fim dele."),
                    new("Medidas de tendência e dispersão", "Média, mediana, amplitude e desvio-padrão.",
                        "A média diz onde o processo está; o desvio-padrão diz o quanto ele varia. Dois processos com a mesma média podem ter qualidades muito diferentes.\n\nA mediana é menos sensível a valores extremos e ajuda a detectar medições fora do padrão."),
                ]),
                new("Controle estatístico de processo", [
                    new("Cartas de controle", "Limites de controle e sinais de processo instável.",
                        "A carta de controle acompanha a média das amostras ao longo do tempo, com limites calculados a partir do próprio processo.\n\nPonto fora do limite, ou sete pontos seguidos do mesmo lado da média, são sinais de causa especial a investigar."),
                    new("Capacidade do processo", "Cp e Cpk explicados com exemplos da indústria.",
                        "Cp compara a tolerância com a variação do processo; Cpk considera também se o processo está centralizado.\n\nUm Cpk abaixo de 1,33 costuma indicar que o processo produz peças fora da especificação com frequência."),
                ]),
                new("Prática presencial", [
                    new("Coleta em laboratório", "Medição real de peças e montagem da carta de controle.",
                        "No encontro presencial, cada grupo mede um lote de peças, registra os dados e monta a carta de controle à mão.\n\nA discussão final compara as cartas dos grupos e as causas de variação encontradas."),
                ]),
            ]),
        new("fisica-movimento-energia-metalurgia",
            "Física do Movimento e da Energia na Metalurgia", "Física na Metalurgia",
            "Estudo de forças, atrito, calor e energia em máquinas, fornos e prensas.",
            "Os conceitos de movimento, força e energia aplicados ao que acontece na planta metalúrgica: esteiras, prensas, fornos e o desgaste das máquinas.",
            3, Modalidade.Presencial, 48, "Trabalhadores da operação e da manutenção de máquinas na metalurgia.",
            StatusCurso.InscricoesAbertas, "hero-image.webp", new DateOnly(2026, 11, 9),
            [
                new("Movimento e força", [
                    new("Velocidade e aceleração", "O movimento das esteiras e pontes rolantes.",
                        "Velocidade é distância por tempo; aceleração é a variação da velocidade. Uma ponte rolante que freia de repente balança a carga.\n\nCalcular a aceleração ajuda a definir rampas de partida e parada mais seguras."),
                    new("Força e atrito", "Por que as peças aquecem e se desgastam.",
                        "O atrito se opõe ao movimento e transforma energia em calor. É útil no freio e prejudicial no mancal.\n\nLubrificação correta reduz o atrito, a temperatura e a troca de componentes."),
                ]),
                new("Energia e calor", [
                    new("Trabalho e potência das prensas", "Quanta energia uma prensa usa a cada golpe.",
                        "Trabalho é força vezes deslocamento. Potência é esse trabalho dividido pelo tempo.\n\nConhecer a potência necessária evita superdimensionar motores e desperdiçar energia."),
                    new("Calor nos fornos", "Temperatura, condução e perdas térmicas.",
                        "O forno transfere calor por condução, convecção e radiação. O isolamento térmico define quanto dessa energia se perde.\n\n## Segurança\n\nSuperfícies acima de 60 °C exigem proteção: o calor que se perde também queima."),
                ]),
            ]),
        new("quimica-sustentabilidade",
            "Química e Sustentabilidade", "Química Sustentável",
            "Estudo das reações químicas envolvidas na tinturaria e acabamento de tecidos.",
            "Como corantes, auxiliares e banhos químicos funcionam na tinturaria, e como reduzir o consumo de água, energia e produtos sem perder a qualidade do acabamento.",
            3, Modalidade.Ead, 36, "Profissionais de tinturaria, lavanderia industrial e acabamento têxtil.",
            StatusCurso.InscricoesAbertas, "banner-ava.webp", new DateOnly(2026, 11, 3),
            [
                new("Reações na tinturaria", [
                    new("Corantes e fixação", "Como a cor se liga à fibra.",
                        "Cada fibra pede uma família de corantes: reativos para algodão, dispersos para poliéster, ácidos para lã e náilon.\n\nA fixação depende de temperatura, tempo e auxiliares químicos no banho."),
                    new("pH do banho", "O controle que define a cor final.",
                        "O pH muda a velocidade da reação e o aproveitamento do corante. Fora da faixa, a cor sai manchada ou mais clara.\n\nMedir o pH no começo e no fim do banho é a forma mais barata de evitar retrabalho."),
                ]),
                new("Acabamento responsável", [
                    new("Tratamento de efluentes", "O que sai do processo e como tratar.",
                        "Efluente de tinturaria carrega cor, sal e carga orgânica. O tratamento combina etapas físicas, químicas e biológicas.\n\nReduzir o volume de banho reduz também o volume de efluente a tratar."),
                    new("Química verde", "Trocar produtos e processos por alternativas menos agressivas.",
                        "Química verde busca reações com menos resíduos e menos substâncias perigosas.\n\nNa prática: corantes de alta fixação, banhos mais curtos e reaproveitamento de água de enxágue."),
                ]),
            ]),
        new("ciencia-materiais-metalurgia",
            "Ciência dos Materiais na Metalurgia", "Ciência dos Materiais",
            "Conceitos básicos de estrutura, resistência e propriedades dos metais.",
            "Da estrutura atômica às propriedades que importam na fábrica: por que um aço é mais duro que outro, como os tratamentos térmicos mudam o material e como os ensaios comprovam isso.",
            3, Modalidade.Presencial, 64, "Técnicos de metalurgia, mecânica e controle de qualidade.",
            StatusCurso.Encerrado, "video-thumb.webp", null,
            [
                new("Estrutura dos metais", [
                    new("Arranjo atômico", "Redes cristalinas e o que elas explicam.",
                        "Os átomos de um metal se organizam em redes cristalinas. O tipo de rede influencia ductilidade e resistência.\n\nDefeitos nessa rede, longe de serem só problemas, são o que permite deformar o metal sem quebrá-lo."),
                    new("Ligas metálicas", "Aço, alumínio e o papel de cada elemento.",
                        "Liga é a mistura de um metal com outros elementos. No aço, o teor de carbono muda dureza e soldabilidade.\n\nCromo e níquel dão resistência à corrosão; é a base do aço inoxidável."),
                ]),
                new("Propriedades e ensaios", [
                    new("Ensaio de tração", "Limite de escoamento e resistência máxima.",
                        "No ensaio de tração, um corpo de prova é esticado até romper. O gráfico tensão × deformação mostra o comportamento do material.\n\nO limite de escoamento indica até onde o material volta ao formato original."),
                    new("Dureza", "Brinell, Rockwell e Vickers.",
                        "Dureza é a resistência à penetração. Cada escala usa um penetrador e uma carga diferentes.\n\nComparar valores de escalas diferentes exige tabela de conversão, nunca conta direta."),
                ]),
                new("Laboratório", [
                    new("Ensaio prático", "Identificação de materiais por ensaios simples.",
                        "Em laboratório, as turmas fazem ensaios de dureza e observam amostras ao microscópio.\n\nO relatório final relaciona a estrutura observada com as propriedades medidas."),
                ]),
            ]),
        new("comunicacao-tecnica-redacao-profissional",
            "Comunicação Técnica e Redação Profissional", "Redação Profissional",
            "Escrita clara de relatórios, e-mails e procedimentos operacionais, com foco em objetividade e revisão.",
            "Escrever bem no trabalho é escrever para ser entendido na primeira leitura. O curso trabalha relatórios, e-mails e procedimentos operacionais com foco em clareza, objetividade e revisão.",
            2, Modalidade.Ead, 30, "Profissionais que redigem relatórios, procedimentos ou comunicados no trabalho.",
            StatusCurso.InscricoesAbertas, "hero-image.webp", new DateOnly(2026, 11, 3),
            [
                new("Escrever para ser entendido", [
                    new("Clareza e objetividade", "Frases curtas, voz ativa e uma ideia por parágrafo.",
                        "Texto técnico bom é o que não precisa ser relido. Frases curtas e voz ativa deixam claro quem faz o quê.\n\nUma ideia por parágrafo facilita a leitura de quem só passa os olhos."),
                    new("Adequação ao leitor", "Ajustar o texto para gestor, colega ou cliente.",
                        "O gestor quer a conclusão primeiro; o colega precisa do passo a passo; o cliente, da solução.\n\nAntes de escrever, responda: quem vai ler e o que essa pessoa precisa decidir?"),
                ]),
                new("Gêneros do trabalho", [
                    new("Relatório técnico", "Estrutura, dados e conclusão que sustenta a decisão.",
                        "Um relatório técnico tem objetivo, método, resultados e conclusão. A conclusão responde ao objetivo.\n\nDados sem interpretação não são relatório; interpretação sem dados não é técnica."),
                    new("Procedimento operacional", "Passo a passo verificável e sem ambiguidade.",
                        "Cada passo do procedimento começa com um verbo e descreve uma única ação.\n\nTeste o procedimento com alguém que não conhece a tarefa: onde a pessoa travar, o texto falhou."),
                ]),
            ]),
        new("economia-circular-industria",
            "Economia Circular na Indústria", "Economia Circular",
            "Princípios da economia circular, ciclo de vida do produto e exemplos de redução de desperdício na produção.",
            "O modelo extrair-produzir-descartar tem limite. O curso apresenta os princípios da economia circular e como aplicá-los no projeto, na produção e no pós-uso dos produtos.",
            4, Modalidade.Ead, 20, "Qualquer pessoa interessada em sustentabilidade no ambiente de trabalho.",
            StatusCurso.InscricoesAbertas, "banner-ava.webp", new DateOnly(2026, 11, 3),
            [
                new("Do linear ao circular", [
                    new("O modelo extrair-produzir-descartar", "Limites do modelo linear de produção.",
                        "No modelo linear, o recurso vira produto e depois lixo. Matéria-prima cara e aterros cheios mostram o limite desse caminho.\n\nA economia circular trata o resíduo como recurso para outro processo."),
                    new("Os princípios da circularidade", "Reduzir, reutilizar, remanufaturar e reciclar.",
                        "A ordem importa: reduzir vem antes de reciclar, porque evita o resíduo em vez de tratá-lo.\n\nRemanufatura devolve o produto ao uso com desempenho de novo, gastando muito menos material."),
                ]),
                new("Na prática", [
                    new("Ciclo de vida do produto", "Mapear impactos do projeto ao descarte.",
                        "A análise de ciclo de vida soma os impactos de cada etapa: extração, fabricação, transporte, uso e descarte.\n\nDecisões de projeto definem a maior parte desse impacto, antes da primeira peça ser produzida."),
                ]),
            ]),
        new("gestao-residuos-eficiencia-energetica",
            "Gestão de Resíduos e Eficiência Energética", "Gestão de Resíduos",
            "Classificação de resíduos, logística reversa e ações de eficiência energética com retorno mensurável.",
            "Resíduo e energia desperdiçada são custo. O curso ensina a classificar e destinar resíduos corretamente e a montar um plano de eficiência energética com retorno mensurável.",
            4, Modalidade.Hibrido, 64, "Supervisores, técnicos de meio ambiente e membros de comissões internas.",
            StatusCurso.Encerrado, "video-thumb.webp", null,
            [
                new("Resíduos", [
                    new("Classificação e armazenamento", "Classes de resíduos e cuidados de armazenamento.",
                        "Resíduos industriais são classificados em perigosos e não perigosos. A classe define armazenamento, transporte e destino.\n\nMisturar classes encarece a destinação de tudo o que foi misturado."),
                    new("Logística reversa", "Responsabilidade compartilhada e destinação correta.",
                        "Na logística reversa, o produto volta ao fabricante depois do uso para reaproveitamento ou destinação correta.\n\nEmbalagens, pilhas, pneus e eletrônicos já têm sistemas obrigatórios."),
                ]),
                new("Energia", [
                    new("Diagnóstico de consumo", "Ler a conta de energia e achar os maiores consumidores.",
                        "A conta de energia separa consumo, demanda e horário. Entender cada parte mostra onde está o custo.\n\nMedir por equipamento revela que poucos consumidores respondem pela maior parte do gasto."),
                    new("Plano de ação", "Priorizar medidas pelo retorno e pelo custo.",
                        "Cada medida tem custo, economia e prazo de retorno. Começar pelas de retorno rápido financia as seguintes.\n\nO plano só funciona com responsável e data para cada ação."),
                ]),
                new("Projeto final", [
                    new("Visita técnica", "Levantamento em campo e apresentação do plano.",
                        "A turma visita uma unidade industrial, levanta resíduos e consumos e monta um plano de melhoria.\n\nA apresentação final é feita para a equipe da própria unidade visitada."),
                ]),
            ]),
        new("ingles-instrumental-manuais-tecnicos",
            "Inglês Instrumental para Manuais Técnicos", "Inglês Instrumental",
            "Leitura de manuais, catálogos e telas de máquinas em inglês, com foco no vocabulário da produção.",
            "Boa parte dos manuais, catálogos e painéis de máquinas chega em inglês. O curso ensina estratégias de leitura e o vocabulário técnico para entender esses textos sem depender de tradução.",
            2, Modalidade.Ead, 30, "Operadores e técnicos que consultam documentação de equipamentos importados.",
            StatusCurso.InscricoesAbertas, "hero-image.webp", new DateOnly(2026, 9, 14),
            [
                new("Estratégias de leitura", [
                    new("Skimming e scanning", "Achar a ideia geral e a informação exata sem ler tudo.",
                        "Skimming é passar os olhos para entender do que o texto trata; scanning é procurar uma informação específica, como um valor de torque.\n\nNo manual, os dois começam pelo sumário, pelos títulos e pelas tabelas."),
                    new("Cognatos e falsos cognatos", "Palavras parecidas que ajudam e as que enganam.",
                        "Cognatos como \"temperature\" e \"pressure\" aceleram a leitura.\n\n## Atenção\n\nFalsos cognatos enganam: \"actually\" é \"na verdade\", e \"pretend\" é \"fingir\", não \"pretender\"."),
                ]),
                new("O manual da máquina", [
                    new("Avisos de segurança", "Warning, caution e danger: o que cada nível exige.",
                        "Os avisos seguem uma escala: \"notice\" informa, \"caution\" alerta para risco leve, \"warning\" para risco grave e \"danger\" para risco iminente.\n\nReconhecer a palavra-chave basta para saber o quanto parar e reler."),
                    new("Instruções passo a passo", "Verbos no imperativo e a ordem das etapas.",
                        "Instruções usam o imperativo: \"remove\", \"tighten\", \"check\". A ordem dos passos é a ordem da execução.\n\nPalavras como \"before\", \"after\" e \"until\" mudam essa ordem e merecem atenção."),
                ]),
            ]),
        new("leitura-interpretacao-normas-tecnicas",
            "Leitura e Interpretação de Normas Técnicas", "Normas Técnicas",
            "Como uma norma é organizada, o que é requisito e o que é recomendação, e como aplicá-la no posto de trabalho.",
            "Normas técnicas definem como medir, fabricar e inspecionar. O curso mostra a estrutura de uma norma, a diferença entre requisito e recomendação e como transformar o texto em procedimento.",
            2, Modalidade.Ead, 16, "Técnicos de qualidade, inspetores e líderes de produção.",
            StatusCurso.InscricoesAbertas, "banner-ava.webp", new DateOnly(2026, 10, 5),
            [
                new("A estrutura da norma", [
                    new("Escopo, referências e definições", "As primeiras seções dizem onde a norma vale.",
                        "O escopo diz a que produtos e processos a norma se aplica; ler essa seção primeiro evita aplicar a norma errada.\n\nAs definições fixam o sentido de cada termo dentro daquele documento."),
                    new("Requisito e recomendação", "\"Deve\" obriga; \"convém\" orienta.",
                        "Na linguagem normativa, \"deve\" indica requisito: sem ele, não há conformidade. \"Convém\" indica recomendação, e \"pode\" indica permissão.\n\nConfundir os verbos gera auditoria reprovada ou trabalho desnecessário."),
                ]),
                new("Da norma ao procedimento", [
                    new("Transformar requisito em passo", "Escrever o procedimento que cumpre a norma.",
                        "Cada requisito aplicável vira um passo verificável do procedimento, com responsável e registro.\n\n## Exercício\n\nEscolha um requisito de inspeção e escreva o passo correspondente em uma frase."),
                ]),
            ]),
        new("agua-efluentes-industria",
            "Água e Efluentes na Indústria", "Água e Efluentes",
            "Uso racional da água, reúso e tratamento de efluentes para reduzir custo e atender à legislação.",
            "Água é insumo e efluente é custo. O curso cobre o balanço hídrico da fábrica, as oportunidades de reúso e as etapas de tratamento exigidas antes do descarte.",
            4, Modalidade.Presencial, 24, "Técnicos de meio ambiente, operadores de utilidades e supervisores de produção.",
            StatusCurso.InscricoesAbertas, "video-thumb.webp", new DateOnly(2026, 12, 7),
            [
                new("Uso da água", [
                    new("Balanço hídrico", "Quanto entra, quanto sai e onde se perde.",
                        "O balanço hídrico compara a água captada com a consumida em cada processo e a descartada.\n\nA diferença aponta vazamentos e usos que ninguém mede."),
                    new("Reúso", "Onde a água de um processo serve a outro.",
                        "Água de enxágue final pode lavar pisos; condensado pode voltar à caldeira.\n\nO critério é a qualidade exigida pelo uso seguinte, não a origem da água."),
                ]),
                new("Tratamento de efluentes", [
                    new("Etapas do tratamento", "Preliminar, primário, secundário e terciário.",
                        "O tratamento preliminar retira sólidos grosseiros; o primário, material sedimentável; o secundário, matéria orgânica por ação biológica.\n\nO terciário remove o que sobra quando a legislação ou o reúso exigem."),
                    new("Visita à estação", "Acompanhamento da operação de uma estação de tratamento.",
                        "A turma acompanha a coleta de amostras e a leitura dos indicadores da estação.\n\nO relatório final compara os resultados com os limites de lançamento."),
                ]),
            ]),
    ];

    /// <summary>Id que o seed dá ao curso (posição na lista, a partir de 1).</summary>
    public static int IdCurso(string slug) => Array.FindIndex(Cursos, c => c.Slug == slug) + 1;

    /// <summary>Id que o seed dá à aula: a mesma contagem sequencial de <see cref="Aplicar"/>.</summary>
    public static int IdAula(string slug, int modulo, int aula)
    {
        var id = 0;
        foreach (var c in Cursos)
            for (var m = 0; m < c.Modulos.Length; m++)
                for (var a = 0; a < c.Modulos[m].Aulas.Length; a++)
                {
                    id++;
                    if (c.Slug == slug && m + 1 == modulo && a + 1 == aula)
                        return id;
                }
        throw new ArgumentException($"Aula {modulo}.{aula} de '{slug}' não existe no seed.");
    }

    public static void Aplicar(ModelBuilder mb)
    {
        mb.Entity<Categoria>().HasData(Categorias);

        var cursos = new List<object>();
        var modulos = new List<object>();
        var aulas = new List<object>();
        int idModulo = 0, idAula = 0;

        for (var i = 0; i < Cursos.Length; i++)
        {
            var c = Cursos[i];
            var idCurso = i + 1;
            cursos.Add(new
            {
                Id = idCurso, c.Slug, c.NomeCompleto, c.NomeBreve, c.Resumo, c.Descricao, c.CategoriaId, c.Modalidade,
                CargaHorariaHoras = c.CargaHoraria, c.PublicoAlvo, c.Status, ImagemUrl = Img + c.Imagem,
                DataDisponibilizacao = c.Disponibilizacao,
                NomeBusca = TextoBusca.Normalizar($"{c.NomeCompleto} {c.NomeBreve}"),
            });

            for (var m = 0; m < c.Modulos.Length; m++)
            {
                idModulo++;
                modulos.Add(new { Id = idModulo, CursoId = idCurso, Ordem = m + 1, c.Modulos[m].Titulo });
                for (var a = 0; a < c.Modulos[m].Aulas.Length; a++)
                {
                    var aula = c.Modulos[m].Aulas[a];
                    aulas.Add(new { Id = ++idAula, ModuloId = idModulo, Ordem = a + 1, aula.Titulo, aula.Descricao, aula.Conteudo });
                }
            }
        }

        mb.Entity<Curso>().HasData(cursos.ToArray());
        mb.Entity<Modulo>().HasData(modulos.ToArray());
        mb.Entity<Aula>().HasData(aulas.ToArray());
    }
}
