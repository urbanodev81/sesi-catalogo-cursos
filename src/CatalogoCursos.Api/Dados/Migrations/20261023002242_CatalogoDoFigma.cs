using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CatalogoCursos.Api.Dados.Migrations
{
    /// <inheritdoc />
    public partial class CatalogoDoFigma : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // O seed troca de lugar aulas e módulos linha a linha; no meio do caminho dois registros
            // ocupariam a mesma (pai, ordem). Os índices únicos saem e voltam dentro da mesma transação.
            DerrubarIndicesDeOrdem(migrationBuilder);
            migrationBuilder.AddColumn<string>(
                name: "Resumo",
                table: "cursos",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Conteudo",
                table: "aulas",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Conteudo", "Descricao", "Titulo" },
                values: new object[] { "## Boas-vindas\n\nNeste curso, a matemática sai do caderno e vai para a bancada: cada conceito aparece ligado a uma peça, um instrumento ou um desenho técnico da metalurgia.\n\nAo final, você vai ler cotas, converter unidades, escolher o instrumento certo e interpretar tolerâncias com segurança.", "Como o curso funciona e o que você vai medir até o fim dele.", "Introdução" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Conteudo", "Descricao", "Titulo" },
                values: new object[] { "## Boas práticas\n\nSe você está interessado em se aprofundar em Matemática para a Precisão, especialmente em Medidas e Escalas na Metalurgia, aqui estão algumas dicas para maximizar sua experiência online. Primeiro, crie um espaço de estudo livre de distrações, onde você possa se concentrar totalmente no material.\n\nEstabeleça uma rotina de estudos, reservando horários específicos para assistir às aulas e realizar as atividades. Além disso, interaja com seus colegas e instrutores nos fóruns e chats disponíveis, pois isso enriquece o aprendizado e ajuda a esclarecer dúvidas. Não se esqueça de revisar o conteúdo regularmente e aplicar os conhecimentos em situações práticas sempre que possível.\n\nPor fim, mantenha-se atualizado sobre as novas técnicas e normas em medidas e escalas na metalurgia, já que essa área está sempre evoluindo. Com dedicação e as estratégias corretas, você estará preparado para se destacar no setor!\n\n## Recomendações\n\nSe você está interessado em se aprofundar na matemática aplicada à metalurgia, aqui estão algumas dicas valiosas. Primeiro, escolha um curso que ofereça uma abordagem prática, com aulas que incluam exercícios e estudos de caso. Isso ajudará a entender como aplicar medidas e escalas na prática. Além disso, verifique se o curso é reconhecido por instituições relevantes, pois isso pode impactar sua formação e carreira.\n\nOutro aspecto importante é a experiência dos instrutores. Procure cursos que tenham profissionais com experiência no setor, pois eles podem compartilhar insights valiosos que vão além da teoria.\n\nConsidere também a carga horária e a flexibilidade do curso. Se você trabalha, um curso que ofereça aulas noturnas ou aos finais de semana pode ser mais conveniente.\n\nPor fim, não se esqueça de se conectar com outros alunos e profissionais da área. Isso pode abrir portas e proporcionar um aprendizado ainda mais enriquecedor. Investir em um bom curso de matemática para a metalurgia é essencial para garantir precisão e eficiência no trabalho.", "Dicas para aproveitar melhor o estudo online.", "Boas práticas" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Conteudo", "Descricao", "ModuloId", "Ordem", "Titulo" },
                values: new object[] { "## Materiais de apoio\n\nGuarde a tabela de conversão entre milímetro, polegada e micrômetro: ela vai ser usada em quase todas as aulas.\n\nO glossário reúne os termos de metrologia que aparecem no curso, como resolução, exatidão, incerteza e tolerância.", "Materiais de apoio: tabela de conversões e glossário.", 1, 3, "Extras" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Conteudo", "Descricao", "Ordem", "Titulo" },
                values: new object[] { "O Sistema Internacional padroniza as unidades para que uma peça medida em São Paulo tenha a mesma medida em qualquer lugar.\n\nNa metalurgia, o milímetro é a unidade de trabalho: desenhos, instrumentos e tolerâncias usam essa escala.", "Metro, quilograma e segundo no dia a dia da produção.", 1, "O Sistema Internacional na oficina" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Conteudo", "Descricao", "Ordem", "Titulo" },
                values: new object[] { "Uma polegada vale 25,4 mm. Converter é multiplicar ou dividir por esse fator, e o erro mais comum é a vírgula no lugar errado.\n\n## Exercício\n\nConverta 3/8\" para milímetros e confira o resultado com o paquímetro na aula seguinte.", "Milímetro, polegada e micrômetro sem erro de vírgula.", 2, "Conversão de unidades" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Conteudo", "Descricao", "Titulo" },
                values: new object[] { "O paquímetro mede dimensões externas, internas e de profundidade com resolução de 0,05 mm ou 0,02 mm.\n\nA leitura soma a escala principal com o traço do nônio que coincide; apoiar a peça corretamente evita erro de paralaxe.", "Leitura do nônio e cuidados de medição.", "Paquímetro" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Conteudo", "Descricao", "Titulo" },
                values: new object[] { "O micrômetro chega a 0,01 mm e é o instrumento de quem controla ajuste de eixo e furo.\n\nA catraca garante a mesma pressão de medição: apertar o tambor com a mão muda o resultado.", "Quando a precisão precisa ir além do paquímetro.", "Micrômetro" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Conteudo", "Descricao", "Titulo" },
                values: new object[] { "Escala 1:2 significa que o desenho tem metade do tamanho real; 2:1, o dobro. A cota sempre informa a medida real.\n\nMedir o desenho com régua é um erro clássico: a escala serve para a proporção, não para a fabricação.", "Ampliação, redução e leitura de cotas.", "Escalas no desenho técnico" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Conteudo", "Descricao", "Titulo" },
                values: new object[] { "Tolerância é a variação permitida em torno da medida nominal. Um eixo de 20 mm com tolerância de ±0,05 mm é bom entre 19,95 e 20,05 mm.\n\nAjuste é a combinação de tolerâncias de eixo e furo: com folga, incerto ou com interferência.", "Quanto a peça pode variar e continuar boa.", "Tolerâncias e ajustes" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Conteudo", "Descricao", "Titulo" },
                values: new object[] { "Toda estampa corrida é um padrão que se repete: a unidade mínima, chamada rapport, é desenhada uma vez e replicada.\n\nIdentificar os eixos de simetria ajuda a prever como o desenho vai se comportar na peça pronta.", "Eixos de simetria e repetição em estampas.", "Simetria e padrões" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Conteudo", "Descricao", "ModuloId", "Ordem", "Titulo" },
                values: new object[] { "Translação desloca, rotação gira e reflexão espelha. Combinadas, essas três operações geram quase todos os padrões têxteis.\n\n## Prática\n\nMonte um padrão a partir de um único motivo usando só rotações de 90 graus.", "Translação, rotação e reflexão no desenho de estampas.", 5, 2, "Transformações geométricas" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Conteudo", "Descricao", "Ordem", "Titulo" },
                values: new object[] { "O molde é uma figura plana que, costurada, cria volume. Retângulos, trapézios e curvas são calculados a partir das medidas do corpo.\n\nA folga de costura é somada ao contorno, e esquecê-la muda o tamanho final da peça.", "Como medidas do corpo viram figuras planas.", 1, "Do molde à peça" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Conteudo", "Descricao", "ModuloId", "Ordem", "Titulo" },
                values: new object[] { "Encaixe é a arrumação dos moldes sobre o tecido. A área aproveitada dividida pela área total dá o rendimento do corte.\n\nPequenas rotações dos moldes, respeitando o fio do tecido, podem economizar metros em uma produção.", "Posicionar moldes para desperdiçar menos tecido.", 6, 2, "Encaixe e aproveitamento" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Conteudo", "Descricao", "Ordem", "Titulo" },
                values: new object[] { "Medir todas as peças de um lote quase nunca é viável. A amostra precisa representar o lote: aleatória e do tamanho certo.\n\nAmostra tirada só do começo do turno esconde o desgaste da ferramenta no fim dele.", "Como escolher uma amostra representativa do lote.", 1, "Amostra e população" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Conteudo", "Descricao", "ModuloId", "Ordem", "Titulo" },
                values: new object[] { "A média diz onde o processo está; o desvio-padrão diz o quanto ele varia. Dois processos com a mesma média podem ter qualidades muito diferentes.\n\nA mediana é menos sensível a valores extremos e ajuda a detectar medições fora do padrão.", "Média, mediana, amplitude e desvio-padrão.", 7, 2, "Medidas de tendência e dispersão" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Conteudo", "Descricao", "Ordem", "Titulo" },
                values: new object[] { "A carta de controle acompanha a média das amostras ao longo do tempo, com limites calculados a partir do próprio processo.\n\nPonto fora do limite, ou sete pontos seguidos do mesmo lado da média, são sinais de causa especial a investigar.", "Limites de controle e sinais de processo instável.", 1, "Cartas de controle" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Conteudo", "Descricao", "ModuloId", "Ordem", "Titulo" },
                values: new object[] { "Cp compara a tolerância com a variação do processo; Cpk considera também se o processo está centralizado.\n\nUm Cpk abaixo de 1,33 costuma indicar que o processo produz peças fora da especificação com frequência.", "Cp e Cpk explicados com exemplos da indústria.", 8, 2, "Capacidade do processo" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Conteudo", "Descricao", "Ordem", "Titulo" },
                values: new object[] { "No encontro presencial, cada grupo mede um lote de peças, registra os dados e monta a carta de controle à mão.\n\nA discussão final compara as cartas dos grupos e as causas de variação encontradas.", "Medição real de peças e montagem da carta de controle.", 1, "Coleta em laboratório" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Conteudo", "Descricao", "Titulo" },
                values: new object[] { "Velocidade é distância por tempo; aceleração é a variação da velocidade. Uma ponte rolante que freia de repente balança a carga.\n\nCalcular a aceleração ajuda a definir rampas de partida e parada mais seguras.", "O movimento das esteiras e pontes rolantes.", "Velocidade e aceleração" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Conteudo", "Descricao", "Titulo" },
                values: new object[] { "O atrito se opõe ao movimento e transforma energia em calor. É útil no freio e prejudicial no mancal.\n\nLubrificação correta reduz o atrito, a temperatura e a troca de componentes.", "Por que as peças aquecem e se desgastam.", "Força e atrito" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "Conteudo", "Descricao", "Titulo" },
                values: new object[] { "Trabalho é força vezes deslocamento. Potência é esse trabalho dividido pelo tempo.\n\nConhecer a potência necessária evita superdimensionar motores e desperdiçar energia.", "Quanta energia uma prensa usa a cada golpe.", "Trabalho e potência das prensas" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Conteudo", "Descricao", "Titulo" },
                values: new object[] { "O forno transfere calor por condução, convecção e radiação. O isolamento térmico define quanto dessa energia se perde.\n\n## Segurança\n\nSuperfícies acima de 60 °C exigem proteção: o calor que se perde também queima.", "Temperatura, condução e perdas térmicas.", "Calor nos fornos" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Conteudo", "Descricao", "ModuloId", "Ordem", "Titulo" },
                values: new object[] { "Cada fibra pede uma família de corantes: reativos para algodão, dispersos para poliéster, ácidos para lã e náilon.\n\nA fixação depende de temperatura, tempo e auxiliares químicos no banho.", "Como a cor se liga à fibra.", 12, 1, "Corantes e fixação" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "Conteudo", "Descricao", "Ordem", "Titulo" },
                values: new object[] { "O pH muda a velocidade da reação e o aproveitamento do corante. Fora da faixa, a cor sai manchada ou mais clara.\n\nMedir o pH no começo e no fim do banho é a forma mais barata de evitar retrabalho.", "O controle que define a cor final.", 2, "pH do banho" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "Conteudo", "Descricao", "ModuloId", "Ordem", "Titulo" },
                values: new object[] { "Efluente de tinturaria carrega cor, sal e carga orgânica. O tratamento combina etapas físicas, químicas e biológicas.\n\nReduzir o volume de banho reduz também o volume de efluente a tratar.", "O que sai do processo e como tratar.", 13, 1, "Tratamento de efluentes" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "Conteudo", "Descricao", "Ordem", "Titulo" },
                values: new object[] { "Química verde busca reações com menos resíduos e menos substâncias perigosas.\n\nNa prática: corantes de alta fixação, banhos mais curtos e reaproveitamento de água de enxágue.", "Trocar produtos e processos por alternativas menos agressivas.", 2, "Química verde" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "Conteudo", "Descricao", "ModuloId", "Ordem", "Titulo" },
                values: new object[] { "Os átomos de um metal se organizam em redes cristalinas. O tipo de rede influencia ductilidade e resistência.\n\nDefeitos nessa rede, longe de serem só problemas, são o que permite deformar o metal sem quebrá-lo.", "Redes cristalinas e o que elas explicam.", 14, 1, "Arranjo atômico" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "Conteudo", "Descricao", "Ordem", "Titulo" },
                values: new object[] { "Liga é a mistura de um metal com outros elementos. No aço, o teor de carbono muda dureza e soldabilidade.\n\nCromo e níquel dão resistência à corrosão; é a base do aço inoxidável.", "Aço, alumínio e o papel de cada elemento.", 2, "Ligas metálicas" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "Conteudo", "Descricao", "ModuloId", "Ordem", "Titulo" },
                values: new object[] { "No ensaio de tração, um corpo de prova é esticado até romper. O gráfico tensão × deformação mostra o comportamento do material.\n\nO limite de escoamento indica até onde o material volta ao formato original.", "Limite de escoamento e resistência máxima.", 15, 1, "Ensaio de tração" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "Conteudo", "Descricao", "Ordem", "Titulo" },
                values: new object[] { "Dureza é a resistência à penetração. Cada escala usa um penetrador e uma carga diferentes.\n\nComparar valores de escalas diferentes exige tabela de conversão, nunca conta direta.", "Brinell, Rockwell e Vickers.", 2, "Dureza" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "Conteudo", "Descricao", "Titulo" },
                values: new object[] { "Em laboratório, as turmas fazem ensaios de dureza e observam amostras ao microscópio.\n\nO relatório final relaciona a estrutura observada com as propriedades medidas.", "Identificação de materiais por ensaios simples.", "Ensaio prático" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "Conteudo", "Descricao", "ModuloId", "Ordem", "Titulo" },
                values: new object[] { "Texto técnico bom é o que não precisa ser relido. Frases curtas e voz ativa deixam claro quem faz o quê.\n\nUma ideia por parágrafo facilita a leitura de quem só passa os olhos.", "Frases curtas, voz ativa e uma ideia por parágrafo.", 17, 1, "Clareza e objetividade" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "Conteudo", "Descricao", "Ordem", "Titulo" },
                values: new object[] { "O gestor quer a conclusão primeiro; o colega precisa do passo a passo; o cliente, da solução.\n\nAntes de escrever, responda: quem vai ler e o que essa pessoa precisa decidir?", "Ajustar o texto para gestor, colega ou cliente.", 2, "Adequação ao leitor" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "Conteudo", "Descricao", "ModuloId", "Ordem", "Titulo" },
                values: new object[] { "Um relatório técnico tem objetivo, método, resultados e conclusão. A conclusão responde ao objetivo.\n\nDados sem interpretação não são relatório; interpretação sem dados não é técnica.", "Estrutura, dados e conclusão que sustenta a decisão.", 18, 1, "Relatório técnico" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "Conteudo", "Descricao", "Ordem", "Titulo" },
                values: new object[] { "Cada passo do procedimento começa com um verbo e descreve uma única ação.\n\nTeste o procedimento com alguém que não conhece a tarefa: onde a pessoa travar, o texto falhou.", "Passo a passo verificável e sem ambiguidade.", 2, "Procedimento operacional" });

            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DataDisponibilizacao", "Descricao", "ImagemUrl", "Modalidade", "NomeBreve", "NomeBusca", "NomeCompleto", "PublicoAlvo", "Resumo", "Slug" },
                values: new object[] { new DateOnly(2027, 1, 5), "Explore nosso curso de Matemática para a Precisão: Medidas e Escalas na Metalurgia, perfeito para quem deseja se destacar nesse campo! Este programa abrangente foi desenvolvido para capacitar profissionais com as habilidades e conhecimentos fundamentais para garantir medições precisas e eficientes no ambiente metalúrgico.", "/assets/imagens/banner-ava.webp", "Hibrido", "Medidas e Escalas", "matematica para a precisao: medidas e escalas na metalurgia medidas e escalas", "Matemática para a Precisão: Medidas e Escalas na Metalurgia", "Operadores, ajustadores e técnicos de usinagem com ensino médio em andamento ou concluído.", "Aplicação de grandezas, proporções e escalas em processos de corte, furação e usinagem.", "matematica-precisao-medidas-escalas-metalurgia" });

            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CargaHorariaHoras", "DataDisponibilizacao", "Descricao", "ImagemUrl", "Modalidade", "NomeBreve", "NomeBusca", "NomeCompleto", "PublicoAlvo", "Resumo", "Slug" },
                values: new object[] { 30, new DateOnly(2027, 2, 1), "Neste curso, a geometria vira ferramenta de criação: simetrias, rotações e malhas explicam estampas, e a modelagem plana mostra como o tecido se transforma em peça com o menor desperdício possível.", "/assets/imagens/hero-image.webp", "Ead", "Geometria Têxtil", "geometria e modelagem no design textil geometria textil", "Geometria e Modelagem no Design Têxtil", "Estudantes e profissionais de confecção, estamparia e design de moda.", "Explora formas geométricas, simetria e padrões matemáticos presentes em tecidos e estampas.", "geometria-modelagem-design-textil" });

            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CargaHorariaHoras", "CategoriaId", "DataDisponibilizacao", "Descricao", "Modalidade", "NomeBreve", "NomeBusca", "NomeCompleto", "PublicoAlvo", "Resumo", "Slug", "Status" },
                values: new object[] { 60, 1, new DateOnly(2026, 11, 16), "Amostragem, média, desvio-padrão e cartas de controle para acompanhar a estabilidade de um processo e decidir com base em dados, não em impressão.", "Hibrido", "Estatística na Qualidade", "estatistica e controle de qualidade na producao estatistica na qualidade", "Estatística e Controle de Qualidade na Produção", "Inspetores de qualidade, líderes de produção e técnicos de processo.", "Leitura e interpretação de dados de controle de qualidade e produtividade.", "estatistica-controle-qualidade-producao", "EmBreve" });

            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CargaHorariaHoras", "CategoriaId", "DataDisponibilizacao", "Descricao", "NomeBreve", "NomeBusca", "NomeCompleto", "PublicoAlvo", "Resumo", "Slug", "Status" },
                values: new object[] { 48, 3, new DateOnly(2026, 11, 9), "Os conceitos de movimento, força e energia aplicados ao que acontece na planta metalúrgica: esteiras, prensas, fornos e o desgaste das máquinas.", "Física na Metalurgia", "fisica do movimento e da energia na metalurgia fisica na metalurgia", "Física do Movimento e da Energia na Metalurgia", "Trabalhadores da operação e da manutenção de máquinas na metalurgia.", "Estudo de forças, atrito, calor e energia em máquinas, fornos e prensas.", "fisica-movimento-energia-metalurgia", "InscricoesAbertas" });

            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CargaHorariaHoras", "DataDisponibilizacao", "Descricao", "Modalidade", "NomeBreve", "NomeBusca", "NomeCompleto", "PublicoAlvo", "Resumo", "Slug" },
                values: new object[] { 36, new DateOnly(2026, 11, 3), "Como corantes, auxiliares e banhos químicos funcionam na tinturaria, e como reduzir o consumo de água, energia e produtos sem perder a qualidade do acabamento.", "Ead", "Química Sustentável", "quimica e sustentabilidade quimica sustentavel", "Química e Sustentabilidade", "Profissionais de tinturaria, lavanderia industrial e acabamento têxtil.", "Estudo das reações químicas envolvidas na tinturaria e acabamento de tecidos.", "quimica-sustentabilidade" });

            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CargaHorariaHoras", "DataDisponibilizacao", "Descricao", "Modalidade", "NomeBreve", "NomeBusca", "NomeCompleto", "PublicoAlvo", "Resumo", "Slug", "Status" },
                values: new object[] { 64, null, "Da estrutura atômica às propriedades que importam na fábrica: por que um aço é mais duro que outro, como os tratamentos térmicos mudam o material e como os ensaios comprovam isso.", "Presencial", "Ciência dos Materiais", "ciencia dos materiais na metalurgia ciencia dos materiais", "Ciência dos Materiais na Metalurgia", "Técnicos de metalurgia, mecânica e controle de qualidade.", "Conceitos básicos de estrutura, resistência e propriedades dos metais.", "ciencia-materiais-metalurgia", "Encerrado" });

            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CargaHorariaHoras", "CategoriaId", "DataDisponibilizacao", "Descricao", "NomeBreve", "NomeBusca", "NomeCompleto", "PublicoAlvo", "Resumo", "Slug" },
                values: new object[] { 30, 2, new DateOnly(2026, 11, 3), "Escrever bem no trabalho é escrever para ser entendido na primeira leitura. O curso trabalha relatórios, e-mails e procedimentos operacionais com foco em clareza, objetividade e revisão.", "Redação Profissional", "comunicacao tecnica e redacao profissional redacao profissional", "Comunicação Técnica e Redação Profissional", "Profissionais que redigem relatórios, procedimentos ou comunicados no trabalho.", "Escrita clara de relatórios, e-mails e procedimentos operacionais, com foco em objetividade e revisão.", "comunicacao-tecnica-redacao-profissional" });

            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CargaHorariaHoras", "DataDisponibilizacao", "Descricao", "Modalidade", "NomeBreve", "NomeBusca", "NomeCompleto", "PublicoAlvo", "Resumo", "Slug", "Status" },
                values: new object[] { 20, new DateOnly(2026, 11, 3), "O modelo extrair-produzir-descartar tem limite. O curso apresenta os princípios da economia circular e como aplicá-los no projeto, na produção e no pós-uso dos produtos.", "Ead", "Economia Circular", "economia circular na industria economia circular", "Economia Circular na Indústria", "Qualquer pessoa interessada em sustentabilidade no ambiente de trabalho.", "Princípios da economia circular, ciclo de vida do produto e exemplos de redução de desperdício na produção.", "economia-circular-industria", "InscricoesAbertas" });

            migrationBuilder.InsertData(
                table: "cursos",
                columns: new[] { "Id", "CargaHorariaHoras", "CategoriaId", "DataDisponibilizacao", "Descricao", "ImagemUrl", "Modalidade", "NomeBreve", "NomeBusca", "NomeCompleto", "PublicoAlvo", "Resumo", "Slug", "Status" },
                values: new object[] { 9, 64, 4, null, "Resíduo e energia desperdiçada são custo. O curso ensina a classificar e destinar resíduos corretamente e a montar um plano de eficiência energética com retorno mensurável.", "/assets/imagens/video-thumb.webp", "Hibrido", "Gestão de Resíduos", "gestao de residuos e eficiencia energetica gestao de residuos", "Gestão de Resíduos e Eficiência Energética", "Supervisores, técnicos de meio ambiente e membros de comissões internas.", "Classificação de resíduos, logística reversa e ações de eficiência energética com retorno mensurável.", "gestao-residuos-eficiencia-energetica", "Encerrado" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 1,
                column: "Titulo",
                value: "Introdução");

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 2,
                column: "Titulo",
                value: "Grandezas e unidades");

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CursoId", "Ordem", "Titulo" },
                values: new object[] { 1, 3, "Instrumentos de medição" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CursoId", "Ordem", "Titulo" },
                values: new object[] { 1, 4, "Escalas e tolerâncias" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Ordem", "Titulo" },
                values: new object[] { 1, "Formas no tecido" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CursoId", "Ordem", "Titulo" },
                values: new object[] { 2, 2, "Modelagem plana" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Ordem", "Titulo" },
                values: new object[] { 1, "Fundamentos de estatística" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CursoId", "Ordem", "Titulo" },
                values: new object[] { 3, 2, "Controle estatístico de processo" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CursoId", "Ordem", "Titulo" },
                values: new object[] { 3, 3, "Prática presencial" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CursoId", "Titulo" },
                values: new object[] { 4, "Movimento e força" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CursoId", "Titulo" },
                values: new object[] { 4, "Energia e calor" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CursoId", "Titulo" },
                values: new object[] { 5, "Reações na tinturaria" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CursoId", "Titulo" },
                values: new object[] { 5, "Acabamento responsável" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CursoId", "Titulo" },
                values: new object[] { 6, "Estrutura dos metais" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CursoId", "Titulo" },
                values: new object[] { 6, "Propriedades e ensaios" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CursoId", "Ordem", "Titulo" },
                values: new object[] { 6, 3, "Laboratório" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CursoId", "Ordem", "Titulo" },
                values: new object[] { 7, 1, "Escrever para ser entendido" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "CursoId", "Ordem", "Titulo" },
                values: new object[] { 7, 2, "Gêneros do trabalho" });

            migrationBuilder.InsertData(
                table: "modulos",
                columns: new[] { "Id", "CursoId", "Ordem", "Titulo" },
                values: new object[,]
                {
                    { 19, 8, 1, "Do linear ao circular" },
                    { 20, 8, 2, "Na prática" }
                });

            migrationBuilder.InsertData(
                table: "aulas",
                columns: new[] { "Id", "Conteudo", "Descricao", "ModuloId", "Ordem", "Titulo" },
                values: new object[,]
                {
                    { 36, "No modelo linear, o recurso vira produto e depois lixo. Matéria-prima cara e aterros cheios mostram o limite desse caminho.\n\nA economia circular trata o resíduo como recurso para outro processo.", "Limites do modelo linear de produção.", 19, 1, "O modelo extrair-produzir-descartar" },
                    { 37, "A ordem importa: reduzir vem antes de reciclar, porque evita o resíduo em vez de tratá-lo.\n\nRemanufatura devolve o produto ao uso com desempenho de novo, gastando muito menos material.", "Reduzir, reutilizar, remanufaturar e reciclar.", 19, 2, "Os princípios da circularidade" },
                    { 38, "A análise de ciclo de vida soma os impactos de cada etapa: extração, fabricação, transporte, uso e descarte.\n\nDecisões de projeto definem a maior parte desse impacto, antes da primeira peça ser produzida.", "Mapear impactos do projeto ao descarte.", 20, 1, "Ciclo de vida do produto" }
                });

            migrationBuilder.InsertData(
                table: "modulos",
                columns: new[] { "Id", "CursoId", "Ordem", "Titulo" },
                values: new object[,]
                {
                    { 21, 9, 1, "Resíduos" },
                    { 22, 9, 2, "Energia" },
                    { 23, 9, 3, "Projeto final" }
                });

            migrationBuilder.InsertData(
                table: "aulas",
                columns: new[] { "Id", "Conteudo", "Descricao", "ModuloId", "Ordem", "Titulo" },
                values: new object[,]
                {
                    { 39, "Resíduos industriais são classificados em perigosos e não perigosos. A classe define armazenamento, transporte e destino.\n\nMisturar classes encarece a destinação de tudo o que foi misturado.", "Classes de resíduos e cuidados de armazenamento.", 21, 1, "Classificação e armazenamento" },
                    { 40, "Na logística reversa, o produto volta ao fabricante depois do uso para reaproveitamento ou destinação correta.\n\nEmbalagens, pilhas, pneus e eletrônicos já têm sistemas obrigatórios.", "Responsabilidade compartilhada e destinação correta.", 21, 2, "Logística reversa" },
                    { 41, "A conta de energia separa consumo, demanda e horário. Entender cada parte mostra onde está o custo.\n\nMedir por equipamento revela que poucos consumidores respondem pela maior parte do gasto.", "Ler a conta de energia e achar os maiores consumidores.", 22, 1, "Diagnóstico de consumo" },
                    { 42, "Cada medida tem custo, economia e prazo de retorno. Começar pelas de retorno rápido financia as seguintes.\n\nO plano só funciona com responsável e data para cada ação.", "Priorizar medidas pelo retorno e pelo custo.", 22, 2, "Plano de ação" },
                    { 43, "A turma visita uma unidade industrial, levanta resíduos e consumos e monta um plano de melhoria.\n\nA apresentação final é feita para a equipe da própria unidade visitada.", "Levantamento em campo e apresentação do plano.", 23, 1, "Visita técnica" }
                });
            RecriarIndicesDeOrdem(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // O seed troca de lugar aulas e módulos linha a linha; no meio do caminho dois registros
            // ocupariam a mesma (pai, ordem). Os índices únicos saem e voltam dentro da mesma transação.
            DerrubarIndicesDeOrdem(migrationBuilder);
            migrationBuilder.DeleteData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DropColumn(
                name: "Resumo",
                table: "cursos");

            migrationBuilder.DropColumn(
                name: "Conteudo",
                table: "aulas");

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Descricao", "Titulo" },
                values: new object[] { "Como calcular rendimento, perdas e consumo por peça.", "Razão e proporção na produção" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Descricao", "Titulo" },
                values: new object[] { "Leitura de OEE, refugo e meta de produção.", "Porcentagem e indicadores" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Descricao", "ModuloId", "Ordem", "Titulo" },
                values: new object[] { "Relação linear entre leitura do sensor e grandeza medida.", 2, 1, "Função afim na calibração" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Descricao", "Ordem", "Titulo" },
                values: new object[] { "Interpretar séries de dados vindas de máquinas conectadas.", 2, "Gráficos de tendência" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Descricao", "Ordem", "Titulo" },
                values: new object[] { "Montar um painel simples de acompanhamento de linha.", 3, "Estudo de caso" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Descricao", "Titulo" },
                values: new object[] { "Como escolher uma amostra representativa do lote.", "Amostra e população" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Descricao", "Titulo" },
                values: new object[] { "Média, mediana, amplitude e desvio-padrão.", "Medidas de tendência e dispersão" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Descricao", "Titulo" },
                values: new object[] { "Limites de controle e sinais de processo instável.", "Cartas de controle" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Descricao", "Titulo" },
                values: new object[] { "Cp e Cpk explicados com exemplos da indústria.", "Capacidade do processo" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Descricao", "Titulo" },
                values: new object[] { "Medição real de peças e montagem da carta de controle.", "Coleta em laboratório" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Descricao", "ModuloId", "Ordem", "Titulo" },
                values: new object[] { "Frases curtas, voz ativa e uma ideia por parágrafo.", 6, 1, "Clareza e objetividade" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Descricao", "Ordem", "Titulo" },
                values: new object[] { "Ajustar o texto para gestor, colega ou cliente.", 2, "Adequação ao leitor" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Descricao", "ModuloId", "Ordem", "Titulo" },
                values: new object[] { "Estrutura, dados e conclusão que sustenta a decisão.", 7, 1, "Relatório técnico" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Descricao", "Ordem", "Titulo" },
                values: new object[] { "Passo a passo verificável e sem ambiguidade.", 2, "Procedimento operacional" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Descricao", "ModuloId", "Ordem", "Titulo" },
                values: new object[] { "Achar a informação certa sem traduzir o texto inteiro.", 8, 1, "Skimming e scanning" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Descricao", "Ordem", "Titulo" },
                values: new object[] { "Palavras que ajudam e as que enganam.", 2, "Cognatos e falsos cognatos" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Descricao", "ModuloId", "Ordem", "Titulo" },
                values: new object[] { "Termos de segurança, ferramentas e procedimentos.", 9, 1, "Manuais de manutenção" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Descricao", "Ordem", "Titulo" },
                values: new object[] { "Mensagens de alarme e menus de IHM.", 2, "Interfaces de máquinas" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Descricao", "Titulo" },
                values: new object[] { "Iônica, covalente e metálica, e o que cada uma explica.", "Ligações químicas" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Descricao", "Titulo" },
                values: new object[] { "Dureza, condutividade e resistência à corrosão.", "Propriedades dos materiais" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "Descricao", "Titulo" },
                values: new object[] { "Aço, alumínio e tratamentos térmicos.", "Metais e ligas" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Descricao", "Titulo" },
                values: new object[] { "Termoplásticos, termofixos e reciclagem.", "Polímeros" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Descricao", "ModuloId", "Ordem", "Titulo" },
                values: new object[] { "Ensaios simples de identificação de materiais.", 11, 3, "Laboratório" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "Descricao", "Ordem", "Titulo" },
                values: new object[] { "Alavancas, engrenagens e transmissão de movimento.", 1, "Força e torque" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "Descricao", "ModuloId", "Ordem", "Titulo" },
                values: new object[] { "Por que a máquina aquece e como reduzir perdas.", 12, 2, "Atrito e lubrificação" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "Descricao", "Ordem", "Titulo" },
                values: new object[] { "Calcular a potência necessária para uma tarefa.", 1, "Trabalho e potência" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "Descricao", "ModuloId", "Ordem", "Titulo" },
                values: new object[] { "Onde a energia se perde num motor elétrico.", 13, 2, "Eficiência energética" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "Descricao", "Ordem", "Titulo" },
                values: new object[] { "Limites do modelo linear de produção.", 1, "O modelo extrair-produzir-descartar" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "Descricao", "ModuloId", "Ordem", "Titulo" },
                values: new object[] { "Reduzir, reutilizar, remanufaturar e reciclar.", 14, 2, "Os princípios da circularidade" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "Descricao", "Ordem", "Titulo" },
                values: new object[] { "Mapear impactos do projeto ao descarte.", 1, "Ciclo de vida do produto" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "Descricao", "Titulo" },
                values: new object[] { "Classes de resíduos e cuidados de armazenamento.", "Classificação e armazenamento" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "Descricao", "ModuloId", "Ordem", "Titulo" },
                values: new object[] { "Responsabilidade compartilhada e destinação correta.", 16, 2, "Logística reversa" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "Descricao", "Ordem", "Titulo" },
                values: new object[] { "Ler a conta de energia e achar os maiores consumidores.", 1, "Diagnóstico de consumo" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "Descricao", "ModuloId", "Ordem", "Titulo" },
                values: new object[] { "Priorizar medidas pelo retorno e pelo custo.", 17, 2, "Plano de ação" });

            migrationBuilder.UpdateData(
                table: "aulas",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "Descricao", "Ordem", "Titulo" },
                values: new object[] { "Levantamento em campo e apresentação do plano.", 1, "Visita técnica" });

            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DataDisponibilizacao", "Descricao", "ImagemUrl", "Modalidade", "NomeBreve", "NomeBusca", "NomeCompleto", "PublicoAlvo", "Slug" },
                values: new object[] { null, "Razão, proporção, funções e leitura de gráficos aplicadas a indicadores de produção, sensores e automação.", "/assets/imagens/hero-image.webp", "Ead", "Matemática 4.0", "matematica aplicada a industria 4.0 matematica 4.0", "Matemática Aplicada à Indústria 4.0", "Trabalhadores da indústria com ensino médio em andamento ou concluído.", "matematica-aplicada-industria-4-0" });

            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CargaHorariaHoras", "DataDisponibilizacao", "Descricao", "ImagemUrl", "Modalidade", "NomeBreve", "NomeBusca", "NomeCompleto", "PublicoAlvo", "Slug" },
                values: new object[] { 60, new DateOnly(2026, 11, 16), "Amostragem, média, desvio-padrão e cartas de controle para acompanhar a estabilidade de um processo.", "/assets/imagens/banner-ava.webp", "Hibrido", "Estatística na Qualidade", "estatistica para controle de qualidade estatistica na qualidade", "Estatística para Controle de Qualidade", "Inspetores de qualidade, líderes de produção e técnicos de processo.", "estatistica-controle-qualidade" });

            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CargaHorariaHoras", "CategoriaId", "DataDisponibilizacao", "Descricao", "Modalidade", "NomeBreve", "NomeBusca", "NomeCompleto", "PublicoAlvo", "Slug", "Status" },
                values: new object[] { 30, 2, null, "Escrita clara de relatórios, e-mails e procedimentos operacionais, com foco em objetividade e revisão.", "Ead", "Redação Profissional", "comunicacao tecnica e redacao profissional redacao profissional", "Comunicação Técnica e Redação Profissional", "Profissionais que redigem relatórios, procedimentos ou comunicados no trabalho.", "comunicacao-tecnica-redacao-profissional", "InscricoesAbertas" });

            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CargaHorariaHoras", "CategoriaId", "DataDisponibilizacao", "Descricao", "NomeBreve", "NomeBusca", "NomeCompleto", "PublicoAlvo", "Slug", "Status" },
                values: new object[] { 80, 2, null, "Leitura de manuais, fichas técnicas e telas de equipamentos em inglês, com vocabulário do setor industrial.", "Inglês Instrumental", "ingles instrumental para a industria ingles instrumental", "Inglês Instrumental para a Indústria", "Operadores e técnicos que lidam com manuais e equipamentos importados.", "ingles-instrumental-industria", "Encerrado" });

            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CargaHorariaHoras", "DataDisponibilizacao", "Descricao", "Modalidade", "NomeBreve", "NomeBusca", "NomeCompleto", "PublicoAlvo", "Slug" },
                values: new object[] { 48, null, "Propriedades de metais, polímeros e cerâmicos, e como a estrutura química define o uso de cada material.", "Presencial", "Química dos Materiais", "quimica dos materiais industriais quimica dos materiais", "Química dos Materiais Industriais", "Estudantes do ensino médio e técnicos interessados em processos de transformação.", "quimica-materiais-industriais" });

            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CargaHorariaHoras", "DataDisponibilizacao", "Descricao", "Modalidade", "NomeBreve", "NomeBusca", "NomeCompleto", "PublicoAlvo", "Slug", "Status" },
                values: new object[] { 36, new DateOnly(2027, 2, 1), "Força, torque, energia e potência para entender o funcionamento e a eficiência de máquinas industriais.", "Ead", "Física Aplicada", "fisica aplicada a maquinas e equipamentos fisica aplicada", "Física Aplicada a Máquinas e Equipamentos", "Trabalhadores da manutenção e da operação de máquinas.", "fisica-aplicada-maquinas", "EmBreve" });

            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CargaHorariaHoras", "CategoriaId", "DataDisponibilizacao", "Descricao", "NomeBreve", "NomeBusca", "NomeCompleto", "PublicoAlvo", "Slug" },
                values: new object[] { 20, 4, null, "Princípios da economia circular, ciclo de vida do produto e exemplos de redução de desperdício na produção.", "Economia Circular", "economia circular na industria economia circular", "Economia Circular na Indústria", "Qualquer pessoa interessada em sustentabilidade no ambiente de trabalho.", "economia-circular-industria" });

            migrationBuilder.UpdateData(
                table: "cursos",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CargaHorariaHoras", "DataDisponibilizacao", "Descricao", "Modalidade", "NomeBreve", "NomeBusca", "NomeCompleto", "PublicoAlvo", "Slug", "Status" },
                values: new object[] { 64, null, "Classificação de resíduos, logística reversa e ações de eficiência energética com retorno mensurável.", "Hibrido", "Gestão de Resíduos", "gestao de residuos e eficiencia energetica gestao de residuos", "Gestão de Resíduos e Eficiência Energética", "Supervisores, técnicos de meio ambiente e membros de comissões internas.", "gestao-residuos-eficiencia-energetica", "Encerrado" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 1,
                column: "Titulo",
                value: "Números no chão de fábrica");

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 2,
                column: "Titulo",
                value: "Funções e sensores");

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CursoId", "Ordem", "Titulo" },
                values: new object[] { 2, 1, "Fundamentos de estatística" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CursoId", "Ordem", "Titulo" },
                values: new object[] { 2, 2, "Controle estatístico de processo" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Ordem", "Titulo" },
                values: new object[] { 3, "Prática presencial" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CursoId", "Ordem", "Titulo" },
                values: new object[] { 3, 1, "Escrever para ser entendido" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Ordem", "Titulo" },
                values: new object[] { 2, "Gêneros do trabalho" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CursoId", "Ordem", "Titulo" },
                values: new object[] { 4, 1, "Estratégias de leitura" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CursoId", "Ordem", "Titulo" },
                values: new object[] { 4, 2, "Vocabulário técnico" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CursoId", "Titulo" },
                values: new object[] { 5, "Estrutura da matéria" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CursoId", "Titulo" },
                values: new object[] { 5, "Famílias de materiais" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CursoId", "Titulo" },
                values: new object[] { 6, "Mecânica" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CursoId", "Titulo" },
                values: new object[] { 6, "Energia" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CursoId", "Titulo" },
                values: new object[] { 7, "Do linear ao circular" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CursoId", "Titulo" },
                values: new object[] { 7, "Na prática" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CursoId", "Ordem", "Titulo" },
                values: new object[] { 8, 1, "Resíduos" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CursoId", "Ordem", "Titulo" },
                values: new object[] { 8, 2, "Energia" });

            migrationBuilder.UpdateData(
                table: "modulos",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "CursoId", "Ordem", "Titulo" },
                values: new object[] { 8, 3, "Projeto final" });
            RecriarIndicesDeOrdem(migrationBuilder);
        }

        private static void DerrubarIndicesDeOrdem(MigrationBuilder mb)
        {
            mb.DropIndex(name: "IX_aulas_ModuloId_Ordem", table: "aulas");
            mb.DropIndex(name: "IX_modulos_CursoId_Ordem", table: "modulos");
        }

        private static void RecriarIndicesDeOrdem(MigrationBuilder mb)
        {
            mb.CreateIndex(name: "IX_aulas_ModuloId_Ordem", table: "aulas", columns: new[] { "ModuloId", "Ordem" }, unique: true);
            mb.CreateIndex(name: "IX_modulos_CursoId_Ordem", table: "modulos", columns: new[] { "CursoId", "Ordem" }, unique: true);
        }
    }
}
