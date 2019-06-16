var baseURL = window.location.protocol +
    "//" + window.location.hostname +
    (window.location.port ? ':'
        + window.location.port : '');

function iniciarBatalha(idBatalha) {



    var postData =
    {
        NacaoExercitoBranco: parseInt(getNacaoExercitoBranco()),
        NacaoExercitoPreto : parseInt(getNacaoExercitoPreto())
    };  

    $.ajax({
        type: 'post',
        data: JSON.stringify(postData),
        dataType: 'json',
        contentType: 'application/json',
        url: baseURL + '/api/BatalhasAPI/IniciarBatalha/' + idBatalha
    })
        .done(
            function (data) {
                window.location.href = "/Batalhas/Tabuleiro/" + idBatalha
            }
        )
        .fail(function (error) {
            swal({
                title: "Alerta",
                text: error.responseText,
                icon: "warning",
            });
        });
}

function getNacaoExercitoBranco() {
    return $('#NacaoExercitoBranco').find(":selected").val();
}

function getNacaoExercitoPreto() {
    return $('#NacaoExercitoPreto').find(":selected").val();
}