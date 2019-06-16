/***
 * Baseado no código 
 * https://www.devmedia.com.br/desenhando-um-tabuleiro-de-damas-em-html-css-js/24591
 * 
 * **/
var ObterBatalha;
var MontarTabuleiro;
$(function () {
    var baseUrl = window.location.protocol + "//" +
        window.location.hostname +
        (window.location.port ? ':' + window.location.port : '');
    var casa_selecionada = null;
    var peca_selecionadaId = null;
    var pecasNoTabuleiro = null;
    var pecaSelecionadaObj = null;
    var pecaElem = null;
    var monitorarJogada = false;

    //Dependendo do tipo de autenticação, via token ou via Cookie.
    //O projeto utilizando .NET Core utiliza Cookies.
    var token = sessionStorage.getItem("accessToken");

    /*
     * Função para obter uma batalha do Servidor.
     * Se obter a batalha com sucesso irá verificar o estado da batalha.
     * E após montar o tabuleiro.
     * @param {boolean} monitoramento No modo monitoramento somente atualizará o tabuleiro quando
     * for a sua vez. Útil para ficar esperando atualização do jogo.
     * */
    ObterBatalha = function (BatalhaId, monitoramento = false) {
        var urlObterBatalha = baseUrl + "/api/BatalhasAPI/" + BatalhaId;
        var headers = {};
        if (token) {
            headers.Authorization = token;
        }
        $.ajax({
            type: 'GET',
            url: urlObterBatalha,
            headers: headers
        })
            .done(function (data) {
                if (!monitoramento || isMeuTurno(data)) {
                    VerificarBatalha(data);
                }
            })
            .fail(
            function (jqXHR, textStatus) {
                alert("Código de Erro: " + jqXHR.status + "\n\n" + jqXHR.responseText);
            });
    }

    /*
     * Função para montar o tabuleiro. 
     * Irá colocar todas as peças com Saude maio que 0 no tabuleiro.
     * */
    MontarTabuleiro = function (batalhaParam) {
        pecasNoTabuleiro = [];
        var batalha = batalhaParam;
        var pecas = batalha.tabuleiro.elementosDoExercito;
        var ExercitoBrancoId = batalha.exercitoBrancoId;
        var ExercitoPretoId = batalha.exercitoPretoId;
        var i;        
        var EmailUsuario = sessionStorage.getItem("EmailUsuario");
        //Esvaziar o tabuleiro antes de montá-lo.
        $("#tabuleiro").empty();
        for (i = 0; i < batalha.tabuleiro.altura; i++) {
            $("#tabuleiro").append("<div id='linha_" + i.toString() + "' class='linha' >");
            pecasNoTabuleiro[i] = [];
            for (j = 0; j < batalha.tabuleiro.largura; j++) {
                var nome_casa = "casa_" + i.toString() + "_" + j.toString();
                var classe = (i % 2 == 0 ? (j % 2 == 0 ? "casa_branca" : "casa_preta") : (j % 2 != 0 ? "casa_branca" : "casa_preta"));
                $("#linha_" + i.toString()).append("<div id='" + nome_casa + "' class='casa " + classe + "' />");
   
                for (x = 0; x < pecas.length; x++) {
                    if (pecas[x].saude <= 0) {
                        continue;
                    }
                    if (pecas[x].posicao.altura == i && pecas[x].posicao.largura == j){
                        pecasNoTabuleiro[i][j] = pecas[x];                    
                        if (pecas[x].exercitoId==ExercitoBrancoId) {
                            $("#" + nome_casa).append("<img src='" + pecas[x].imagem + "' class='peca flipped' id='" + nome_casa.replace("casa", "peca_preta") + "'/>");
                        }
                        else if (pecas[x].exercitoId == ExercitoPretoId) {
                            $("#" + nome_casa).append("<img src='" + pecas[x].imagem + "' class='peca' id='" + nome_casa.replace("casa", "peca_branca") + "'/>");
                        }

                    }                    
                }

            }
        }
        $(".casa").click(function () {
            //Retirando a seleção da casa antiga.
            $("#" + casa_selecionada).removeClass("casa_selecionada");
            //Obtendo o Id.
            casa_selecionada = $(this).attr("id");
            //Adicionando Vermelho na Casa nova.
            $("#" + casa_selecionada).addClass("casa_selecionada");
            //Legenda que mostra informações da casa selecionada.
            $("#info_casa_selecionada").text(casa_selecionada);
            var altura = casa_selecionada.split("_")[1]
            var largura = casa_selecionada.split("_")[2]
            if (pecaElem == null) {
                //Obter o id da imagem selecionada.
                peca_selecionadaId = ObterPecaIDNaCasa(casa_selecionada);
                //Se for nulo
                if (peca_selecionadaId == null) {
                    pecaElem = null;
                    peca_selecionadaId = "NENHUMA PECA SELECIONADA";
                } else {
                    //Guardar a peça selecionada.
                    pecaElem = document.getElementById(peca_selecionadaId);
                    pecaSelecionadaObj = pecasNoTabuleiro[altura][largura];
                }
                //Legenda que mostra informações da peça selecionada.
                $("#info_peca_selecionada").text(peca_selecionadaId.toString());
            } else {
                var posicaopeca = {
                    Altura: altura,
                    Largura: largura
                };                
                if(ObterPecaIDNaCasa(casa_selecionada) == null) {
                    ataque = false;
                } else {
                    ataque = true;
                }

                var movimento = {
                    posicao: posicaopeca,
                    AutorId: ObterExercitoTurno(batalha).usuarioId,
                    BatalhaId: batalha.id,
                    ElementoId: pecaSelecionadaObj.id,
                    TipoMovimento: ataque ? "Atacar" :"Mover"
                };



                if (isMeuTurno(batalha) &&
                    ObterExercitoTurno(batalha).id == pecaSelecionadaObj.exercitoId
                ) {
                    Mover(movimento);
                } else if (!isMeuTurno(batalha)) {
                    alert("Não é a sua vez!");
                } else if (ObterExercitoTurno(batalha).id != pecaSelecionadaObj.exercitoId){
                    alert("Não é o seu exercito!");
                }
            

                pecaElem = null;
                $("#" + casa_selecionada).removeClass("casa_selecionada");
            }
        });

        //Caso não seja a vez do usuário atualizar o tabuleiro a cada um segundo até que seja a vez do usuário.
        if (isMeuTurno(batalha)) {
            if (!monitorarJogada) {
                window.setInterval(function () {
                    ObterBatalha(batalha.id);
                }, 1000);
            }
            monitorarJogada = true;
        } else {
            monitorarJogada = false;
        }



        function ObterExercitoTurno(batalha) {
            return (batalha.turnoId == batalha.exercitoBrancoId) ? batalha.exercitoBranco : batalha.exercitoPreto;
        }

        function isMeuTurno(batalha) {
            ExercitoTurno = ObterExercitoTurno(batalha);
            if (ExercitoTurno.usuario.email == EmailUsuario ||
                ExercitoTurno.usuario.username == EmailUsuario) {
                return true;
            }
            return false;
        }

        function ObterPecaIDNaCasa(casa_selecionada) {
            return $("#" + casa_selecionada).children("img:first").attr("id");
        }

        function Mover(movimento) {           
            $.ajax({
                type: 'POST',
                url: baseUrl + "/api/BatalhasAPI/Jogar",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: JSON.stringify(movimento)
            })
                .done(
                function (data) {
                    MontarTabuleiro(data);
                }
                )
                .fail(
                function (jqXHR, textStatus) {
                    alert("Código de Erro: " + jqXHR.status + "\n\n" + jqXHR.responseText);
                });
        }
    }   

    /**
     * 
     * @param {any} Batalha O objeto batalha. Enviado via JSON do servidor.
     * EstadoBatalhaEnum { NaoIniciado =0, Iniciado =1, Finalizado =10, Cancelado =99}
     */
    function VerificarBatalha(Batalha, monitoramento) {
        if (Batalha.Estado == 0) {
            if (Batalha.ExercitoPretoId == null || Batalha.ExercitoBrancoId == null) {
                if ((Batalha.ExercitoPreto != null &&
                    Batalha.ExercitoPreto.Usuario.Email ==
                    sessionStorage.getItem("EmailUsuario")) ||
                    (Batalha.ExercitoBranco != null &&
                        Batalha.ExercitoBranco.Usuario.Email ==
                        sessionStorage.getItem("EmailUsuario"))
                ) {
                    alert("Somente existe um jogador neste jogo. Favor esperar a entrada do outro jogador");
                } 
                alert("O jogo encontra-se em um estado inconsistente. Por favor, espere.");
            } else {
                alert("Volte ao Lobby para iniciar o jogo.");
            }
        } else {
            //No modo monitoramento somente irá atualizar o tabuleiro quando for a sua vez.
            MontarTabuleiro(Batalha);
            if (Batalha.Estado == 10 || Batalha.Estado == 99) {
                //TODO: Implementar um tratamento para a finalização do jogo.
            }                        
        }
    }

});
