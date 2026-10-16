var beneficiarios = [];
var indiceBeneficiarioEdicao = -1;
var clienteEmEdicao = typeof obj !== "undefined" && obj !== null;

$(document).ready(inicializarFormulario);

function inicializarFormulario() {
    if (clienteEmEdicao) {
        preencherFormularioCliente();
        beneficiarios = obj.Beneficiarios || [];
    }

    $(".cpf").on("input", aplicarMascaraCpf);
    $("#salvarBeneficiario").on("click", salvarBeneficiario);
    $("#cancelarEdicaoBeneficiario").on("click", limparFormularioBeneficiario);
    $("#formCadastro").on("submit", salvarCliente);
    montarGridBeneficiarios();
}

function preencherFormularioCliente() {
    $("#Id").val(obj.Id);
    $("#Nome").val(obj.Nome);
    $("#Sobrenome").val(obj.Sobrenome);
    $("#CPF").val(formatarCpf(obj.CPF));
    $("#Nacionalidade").val(obj.Nacionalidade);
    $("#CEP").val(obj.CEP);
    $("#Estado").val(obj.Estado);
    $("#Cidade").val(obj.Cidade);
    $("#Logradouro").val(obj.Logradouro);
    $("#Email").val(obj.Email);
    $("#Telefone").val(obj.Telefone);
}

function aplicarMascaraCpf() {
    this.value = formatarCpf(this.value);
}

function formatarCpf(valor) {
    var cpf = obterCpfDigitos(valor).substring(0, 11);
    if (cpf.length > 9) {
        return cpf.substring(0, 3) + "." + cpf.substring(3, 6) + "." + cpf.substring(6, 9) + "-" + cpf.substring(9);
    }
    if (cpf.length > 6) {
        return cpf.substring(0, 3) + "." + cpf.substring(3, 6) + "." + cpf.substring(6);
    }
    if (cpf.length > 3) {
        return cpf.substring(0, 3) + "." + cpf.substring(3);
    }
    return cpf;
}

function obterCpfDigitos(valor) {
    return (valor || "").replace(/\D/g, "");
}

function validarCpf(valor) {
    var cpf = obterCpfDigitos(valor);
    if (cpf.length !== 11 || /^([0-9])\1{10}$/.test(cpf)) {
        return false;
    }

    for (var posicao = 9; posicao < 11; posicao++) {
        var soma = 0;
        for (var indice = 0; indice < posicao; indice++) {
            soma += parseInt(cpf.charAt(indice), 10) * (posicao + 1 - indice);
        }
        var digito = (soma * 10) % 11;
        if (digito === 10) {
            digito = 0;
        }
        if (digito !== parseInt(cpf.charAt(posicao), 10)) {
            return false;
        }
    }
    return true;
}

function montarGridBeneficiarios() {
    var tabela = $("#gridBeneficiarios tbody");
    tabela.empty();

    for (var indice = 0; indice < beneficiarios.length; indice++) {
        var beneficiario = beneficiarios[indice];
        var linha = $("<tr>");
        $("<td>").text(formatarCpf(beneficiario.CPF)).appendTo(linha);
        $("<td>").text(beneficiario.Nome).appendTo(linha);

        var acoes = $("<td>");
        criarBotaoBeneficiario("Alterar", "btn-primary", indice).appendTo(acoes);
        criarBotaoBeneficiario("Excluir", "btn-primary", indice).appendTo(acoes);
        linha.append(acoes);
        tabela.append(linha);
    }
}

function criarBotaoBeneficiario(texto, classe, indice) {
    var botao = $("<button type='button'>");
    botao.addClass("btn " + classe + " btn-sm");
    botao.text(texto);
    botao.data("indice", indice);
    botao.on("click", executarAcaoBeneficiario);
    return botao;
}

function executarAcaoBeneficiario() {
    var botao = $(this);
    var indice = botao.data("indice");
    if (botao.text() === "Alterar") {
        iniciarEdicaoBeneficiario(indice);
        return;
    }
    excluirBeneficiario(indice);
}

function iniciarEdicaoBeneficiario(indice) {
    var beneficiario = beneficiarios[indice];
    indiceBeneficiarioEdicao = indice;
    $("#IdBeneficiario").val(beneficiario.Id || 0);
    $("#CPFBeneficiario").val(formatarCpf(beneficiario.CPF));
    $("#NomeBeneficiario").val(beneficiario.Nome);
    $("#salvarBeneficiario").text("Alterar");
    $("#cancelarEdicaoBeneficiario").show();
}

function excluirBeneficiario(indice) {
    beneficiarios.splice(indice, 1);
    limparFormularioBeneficiario();
    montarGridBeneficiarios();
}

function salvarBeneficiario() {
    var cpf = $("#CPFBeneficiario").val();
    var nome = $.trim($("#NomeBeneficiario").val());
    if (!validarCpf(cpf)) {
        exibirMensagem("CPF inválido", "Informe um CPF válido para o beneficiário.");
        return;
    }
    if (!nome) {
        exibirMensagem("Nome obrigatório", "Informe o nome do beneficiário.");
        return;
    }
    if (cpfBeneficiarioDuplicado(cpf)) {
        exibirMensagem("CPF duplicado", "Este CPF já está na lista de beneficiários deste cliente.");
        return;
    }

    var beneficiario = {};
    beneficiario.Id = parseInt($("#IdBeneficiario").val(), 10) || 0;
    beneficiario.CPF = obterCpfDigitos(cpf);
    beneficiario.Nome = nome;

    if (indiceBeneficiarioEdicao >= 0) {
        beneficiarios[indiceBeneficiarioEdicao] = beneficiario;
    } else {
        beneficiarios.push(beneficiario);
    }

    limparFormularioBeneficiario();
    montarGridBeneficiarios();
}

function cpfBeneficiarioDuplicado(cpf) {
    var cpfPesquisado = obterCpfDigitos(cpf);
    for (var indice = 0; indice < beneficiarios.length; indice++) {
        if (indice === indiceBeneficiarioEdicao) {
            continue;
        }
        if (obterCpfDigitos(beneficiarios[indice].CPF) === cpfPesquisado) {
            return true;
        }
    }
    return false;
}

function limparFormularioBeneficiario() {
    indiceBeneficiarioEdicao = -1;
    $("#IdBeneficiario").val("0");
    $("#CPFBeneficiario").val("");
    $("#NomeBeneficiario").val("");
    $("#salvarBeneficiario").text("Incluir");
    $("#cancelarEdicaoBeneficiario").hide();
}

function salvarCliente(evento) {
    evento.preventDefault();
    var cpf = $("#CPF").val();
    if (!validarCpf(cpf)) {
        exibirMensagem("CPF inválido", "Informe um CPF válido para o cliente.");
        return;
    }

    var dados = montarModeloCliente();
    $.ajax({
        url: urlPost,
        method: "POST",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(dados),
        error: tratarErroAoSalvar,
        success: tratarSucessoAoSalvar
    });
}

function montarModeloCliente() {
    var dados = {};
    var campos = $("#formCadastro").serializeArray();
    for (var indice = 0; indice < campos.length; indice++) {
        dados[campos[indice].name] = campos[indice].value;
    }

    dados.Id = parseInt($("#Id").val(), 10) || 0;
    dados.CPF = obterCpfDigitos($("#CPF").val());
    dados.Beneficiarios = [];
    for (var indiceBeneficiario = 0; indiceBeneficiario < beneficiarios.length; indiceBeneficiario++) {
        var beneficiario = beneficiarios[indiceBeneficiario];
        var beneficiarioModel = {};
        beneficiarioModel.Id = beneficiario.Id || 0;
        beneficiarioModel.CPF = obterCpfDigitos(beneficiario.CPF);
        beneficiarioModel.Nome = beneficiario.Nome;
        dados.Beneficiarios.push(beneficiarioModel);
    }
    return dados;
}

function tratarErroAoSalvar(resposta) {
    var mensagem = resposta.responseJSON;
    if (typeof mensagem !== "string") {
        mensagem = "Ocorreu um erro interno no servidor.";
    }
    exibirMensagem("Ocorreu um erro", mensagem);
}

function tratarSucessoAoSalvar(resposta) {
    if (clienteEmEdicao) {
        window.location.href = urlRetorno;
        return;
    }

    $("#formCadastro")[0].reset();
    beneficiarios = [];
    limparFormularioBeneficiario();
    montarGridBeneficiarios();
    exibirMensagem("Sucesso!", resposta);
}

function exibirMensagem(titulo, texto) {
    var modal = $("<div>").addClass("modal fade");
    var dialogo = $("<div>").addClass("modal-dialog");
    var conteudo = $("<div>").addClass("modal-content");
    var cabecalho = $("<div>").addClass("modal-header");
    var fechar = $("<button type='button' data-dismiss='modal'>").addClass("close");
    var tituloModal = $("<h4>").addClass("modal-title");
    var corpo = $("<div>").addClass("modal-body");
    var rodape = $("<div>").addClass("modal-footer");
    var botaoFechar = $("<button type='button' data-dismiss='modal'>").addClass("btn btn-default");

    fechar.html("&times;");
    tituloModal.text(titulo);
    corpo.text(texto);
    botaoFechar.text("Fechar");
    cabecalho.append(fechar, tituloModal);
    rodape.append(botaoFechar);
    conteudo.append(cabecalho, corpo, rodape);
    dialogo.append(conteudo);
    modal.append(dialogo);
    modal.on("hidden.bs.modal", function () {
        modal.remove();
    });
    modal.appendTo("body").modal("show");
}
