jQuery(function ($) {

    $("#BuscaCep").click(function () {

        var cep_code = $("#cep").val();

        if (cep_code.length <= 0) return;

        $.getJSON("https://viacep.com.br/ws/" + cep_code + "/json/?callback=?", function (dados) {

            if (!("erro" in dados)) {
                //Atualiza os campos com os valores da consulta.
                $("#rua").val(dados.logradouro);
                $("#bairro").val(dados.bairro);
                $("#cidade").val(dados.localidade);
                $("#uf").val(dados.uf);
            } //end if.
            else {
                //CEP pesquisado não foi encontrado.
                limpa_formulário_cep();
                alert("CEP não encontrado.");
            }
        });
    });
});