function add_anti_fogery_token(data) {
    data.__RequestVerificationToken = $('[name=__RequestVerificationToken]').val();
    return data;
}

function formatar_msg_aviso(mensagens) {
    var retorno = '';
    for (var i = 0; i < mensagens.length; i++) {
        retorno += '<li>' + mensagens[i] + '</li>';
    }
    return '<ul>' + retorno + '</ul>'
}

function abrir_form(dados) {
    set_dados_form(dados);


    var modal_cadastro = $('#modal_cadastro');

    $('#msg_mensagem_aviso').empty();
    $('#msg_aviso').hide();
    $('#msg_mensagem_aviso').hide();
    $('#msg_erro').hide();

    bootbox.dialog({
        title: tituloPagina,
        message: modal_cadastro
    })
    .on('shown.bs.modal', function () {
        modal_cadastro.show(0, function () {
            set_focus_form();
            
        });
    })
    .on('hidden.bs.modal', function () {
        modal_cadastro.hide().appendTo('body');
    });
}

function criar_linha_grid(dados) {
    return '<tr data-id=' + dados.Id + '>' +
    set_dados_grid(dados) +
    '<td>' +
    '<a id="btn_alterar" class="btn btn-primary btn-alterar" role="button" style="margin-right: 3px"><i class="glyphicon glyphicon-pencil"></i> Alterar</a>' +
    '<a id="btn_excluir" class="btn btn-danger btn-excluir" role="button"><i class="glyphicon glyphicon-trash"></i> Excluir</a>' +
    '</td>' +
    '</tr>';

}

$(document)
.on('click', '#btn_incluir', function () {
    abrir_form(get_dados_inclusao());
})
.on('click', '.btn-alterar', function () {
    var btn = $(this),
        id = btn.closest('tr').attr('data-id'),
        url = urlAlterar,
        param = { 'id': id };

    $.post(url, add_anti_fogery_token(param), function (response) {
        if (response) {
            if (url == "/CadastroUsuario/ObterUsuarioId")
                response.Senha = '@ViewBag.SenhaPadrao';
            abrir_form(response);
        }
    });
})
.on('click', '.btn-excluir', function () {
    var btn = $(this),
       tr = btn.closest('tr'),
       id = tr.attr('data-id'),
       url = urlExclusao,
       param = { 'id': id };

    bootbox.confirm({
        message: "Realmente deseja excluir o item?",
        buttons: {
            confirm: {
                label: 'Sim',
                className: 'btn-danger'
            },
            cancel: {
                label: 'Não',
                className: 'btn-success'
            }
        },
        callback: function (result) {
            if (result) {
                $.post(url, add_anti_fogery_token(param), function (response) {
                    if (response) {

                        tr.remove();
                    }
                });
            }
        }
    });
})
.on('click', '#btn_confirmar', function () {
    var btn = $(this),
       url = urlConfirmar,
       param = get_dados_form();

    $.post(url, add_anti_fogery_token(param), function (response) {
        if (response.Resultado == 'Ok') {
            if (param.Id == 0) {
                param.Id = response.IdSalvo;
                var table = $('#grid_cadastro').find('tbody'),
                    linha = criar_linha_grid(param);

                table.append(linha);
            }
            else {
                var linha = $('#grid_cadastro').find('tr[data-id =' + param.Id + ']').find('td');
                preencher_linha_grid(param, linha);
            }

            $('#modal_cadastro').parents('.bootbox').modal('hide');
        }
        else if (response.Resultado == 'Erro') {
            $('#msg_aviso').hide();
            $('#msg_mensagem_aviso').hide();
            $('#msg_erro').show();
        }
        else if (response.Resultado == 'Aviso') {
            $('#msg_mensagem_aviso').html(formatar_msg_aviso(response.Mensagens));
            $('#msg_aviso').show();
            $('#msg_mensagem_aviso').show();
            $('#msg_erro').hide();
        }
    });
})
.on('click', '.page-item', function () {
    var ddl = $(this),
        pagina = ddl.text(),
        tamPag = $('#ddl_tam_pag').val(),
        url = urlPaginacao,
        param = { 'pagina': pagina, 'tamPag': tamPag };

    $.post(url, add_anti_fogery_token(param), function (response) {
        if (response) {
            var table = $('#grid_cadastro').find('tbody');
            table.empty();

            for (var i = 0; i < response.length; i++) {
                table.append(criar_linha_grid(response[i]));
            }
            ddl.siblings().removeClass('active');
            ddl.addClass('active');
        }
    });
})
.on('change', '#ddl_tam_pag', function () {
    var ddl = $(this),
        tamPag = ddl.val(),
        pagina = 1,
        url = urlPaginacao,
        param = { 'pagina': pagina, 'tamPag': tamPag };

    $.post(url, add_anti_fogery_token(param), function (response) {
        if (response) {
            var table = $('#grid_cadastro').find('tbody');
            table.empty();

            for (var i = 0; i < response.length; i++) {
                table.append(criar_linha_grid(response[i]));
            }
            ddl.siblings().removeClass('active');
            ddl.addClass('active');
        }
    });
});