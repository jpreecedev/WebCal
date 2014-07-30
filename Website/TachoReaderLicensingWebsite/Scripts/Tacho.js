$(function () {

    BindUIEvents();

});

function BindUIEvents() {

    $('#mainAdd').click(function (ev) {
        ev.preventDefault();
        ShowAdd();
    });

    $('#clientDetails form').submit(function (ev) {

        ev.preventDefault();
        ev.stopPropagation();

        var $form = $(this);
        SaveClient($form);

    });

    $(".search-large").keyup(function () {
        refreshClientList();
        //ShowAdd();
    });

    bindClientClicks();

    $('#licenceAdd').click(function (ev) {
        ev.preventDefault();

        var clientID = $('input[id=ID]', '#clientDetails form').val();
        var expiryDate = $('#dp1').val();
        if (clientID && expiryDate) {
            AddLicence(clientID, expiryDate);
            $("#myModal").modal("hide");
        }
    });

    var nowTemp = new Date();
    var now = new Date(nowTemp.getFullYear(), nowTemp.getMonth(), nowTemp.getDate(), 0, 0, 0, 0);
    $('.datepicker').datepicker({
        onRender: function (date) {
            return date.valueOf() < now.valueOf() ? 'disabled' : '';
        },
        format: 'dd/mm/yyyy'
    });

    $("#myModal").on('hidden', function () {
        $('.datepicker').val('');
    });
}



function ShowAdd() {

    var $form = $('#clientDetails form');

    $('#clientDetails h3').text('Add Client');
    $('input[type=submit]', $form).val('Add +');
    $('input[id=Name]', $form).val('');
    $('input[id=ID]', $form).val('00000000-0000-0000-0000-000000000000');
    $('input[id=Name]', $form).focus();
    

    HideLicences();

}

function ShowEdit(clientID) {

    $.get('/Client/Load', { 'ClientAccessID': clientID }, function (data, status, jqXHR) {

        var $form = $('#clientDetails form');

        $('#clientDetails h3').text('Client Details');
        $('input[id=Name]', $form).val(data);
        $('input[id=ID]', $form).val(clientID)
        $('input[type=submit]', $form).val('Update');
        

        refreshLicenceList(clientID);

    });

}

function SaveClient($form) {

    var action = $form.attr('action');

    $.post(action, $form.serialize(), function (data, status, jqXHR) {
        if (data && data !== '00000000-0000-0000-0000-000000000000') {
            $('#clientDetails h3').text('Client Details');
            $('input[type=submit]', $form).val('Update');
            $('input[id=ID]', $form).val(data);
            refreshClientList(data);
            refreshLicenceList(data);
        }
    });

}

function refreshClientList(HighlightID) {

    var filter = $(".search-large").val();
    var currentHighlightID = $('#clientDetails form input[id=ID]').val();

    $.get('/Client/RefreshClientList', { 'SearchFilter': filter }, function (data, status, jqXHR) {
        $('#ClientListPlaceholder').html(data);
        bindClientClicks();

        if (HighlightID) {
            ApplySelectedClientHighlight(HighlightID);
        }
        else {
            ApplySelectedClientHighlight(currentHighlightID);
        }

    });
}

function bindClientClicks() {

    $('li.client').click(function (ev) {
        ev.preventDefault();

        var clientID = $(this).attr('data-cid');
        ApplySelectedClientHighlight(clientID);
        ShowEdit(clientID);
    });

    $('li.client a[rel=delete]').click(function (ev) {
        ev.preventDefault();
        ev.stopPropagation();

        var clientID = $(this).closest('li.client').attr('data-cid');
        DeleteClient(clientID);


    })
}

function ApplySelectedClientHighlight(clientId) {

    $('li.client').removeClass("clientClicked");
    $('li.client[data-cid=' + clientId + ']').addClass("clientClicked");

}

function DeleteClient(clientID) {
    var r = confirm("Are you sure you want to delete this client!");
    if (r == true) {
        var currentEditingID = $('#clientDetails form input[id=ID]').val();

        $.post('/Client/Delete', { 'ID': clientID }, function (data, status, jqXHR) {
            refreshClientList();

            if (clientID === currentEditingID) {
                ShowAdd();
            }
        });
    }
    else {
    }

}

function ShowLicences() {
    $('#licenceList').show();
}

function HideLicences() {
    $('#licenceList').hide();
}

function refreshLicenceList(cid) {
    $.get('/Licence/RefreshLicences', { 'id': cid }, function (data, status, jqXHR) {
        $('#licenceListPlaceholder').html(data);
        ShowLicences();
        bindLicenceClicks();
    });
}

function bindLicenceClicks() {

    $('li.licence a[rel=delete]').click(function (ev) {
        ev.preventDefault();

        var LicenceID = $(this).closest('li.licence').attr('data-lid');
        var clientID = $('input[id=ID]', '#clientDetails form').val();
        DeleteLicence(clientID, LicenceID);
    })

}

function AddLicence(clientID, expiryDate) {
    $.post('/Licence/Add', { 'ClientID': clientID, 'ExpiryDate': expiryDate }, function (data, status, jqXHR) {
        refreshLicenceList(clientID);
    });
}

function DeleteLicence(clientID, LicenceID) {
    var r = confirm("Are you sure you want to delete this client's licence!");
    if (r == true) {
        $.post('/Licence/Delete', { 'AccessID': LicenceID }, function (data, status, jqXHR) {
            refreshLicenceList(clientID);
        });
    }
    else {
    }
}