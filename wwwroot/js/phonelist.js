$('#editNumberForm').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget) // Button that triggered the modal
    // If necessary, you could initiate an AJAX request here (and then do the updating in a callback).
    // Update the modal's content. We'll use jQuery here, but you could use a data binding library or other methods instead.
    var modal = $(this)
    var phoneId = button.data('phone_id')

    var serverAlert = modal.find('.modal-content div#server_alert');
    serverAlert.hide();

    if (phoneId == "new") {
        modal.find('.modal-body input#phone-id').val(0)
        modal.find('.modal-title').text('Add phone number')
        modal.find('.modal-body input#phone-number').val("+")

        modal.find('.modal-content button#saveNumber').text('Add');
    } else {
        var phoneNumber = button.data('phone_number')

        modal.find('.modal-body input#phone-id').val(phoneId)
        modal.find('.modal-title').text('Edit number ' + phoneNumber)
        modal.find('.modal-body input#phone-number').val(PhoneMask(phoneNumber.toString()))
        modal.find('.modal-body select').val(button.data('user_id'))

        modal.find('.modal-content button#saveNumber').text('Save');
    }
})

$('#editUserForm').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget) // Button that triggered the modal

    var modal = $(this)
    var userId = button.data('user_id')

    var serverAlert = modal.find('.modal-content div#server_alert');
    serverAlert.hide();

    if (userId == "new") {
        modal.find('.modal-title').text('Add user')
        modal.find('.modal-body input#user-id').val(0)
        modal.find('.modal-body input#first-name').val('')
        modal.find('.modal-body input#father-name').val('')
        modal.find('.modal-body input#surname').val('')
        modal.find('.modal-body input#address').val('')

        modal.find('.modal-content button#saveUser').text('Add');
    } else {
        var firstName = button.data('user_first_name')
        var fatherName = button.data('user_father_name');
        var surname = button.data('user_surname')

        modal.find('.modal-body input#user-id').val(userId)
        modal.find('.modal-title').text('Edit user ' + surname + ' ' + firstName[0] + '.' + fatherName[0] + '.')

        modal.find('.modal-body input#first-name').val(firstName)
        modal.find('.modal-body input#father-name').val(fatherName)
        modal.find('.modal-body input#surname').val(surname)
        modal.find('.modal-body input#address').val(button.data('address'))

        modal.find('.modal-content button#saveUser').text('Save');
    }
})

$('button#saveNumber').on('click', function () {
    var form = $('#editNumberForm').find('.modal-body form')
    var serverAlert = $('#editNumberForm').find('.modal-content div#server_alert');
    var sendData = PreparePhoneData(form.serializeArray());

    serverAlert.hide();

    if (form.find('input#phone-id').val() == 0) {
        $.post("/index/AjaxNewNumber/", sendData)
            .done(function (data) {
                if (data.result == "true") {
                    $('#editNumberForm').modal("hide");
                    document.location = document.location;
                } else {
                    ServerErrorException(data, serverAlert);
                }
            })
            .fail(function (data) {
                ServerErrorException(data, serverAlert);
            });
    } else {
        $.post("/index/AjaxEditNumber/", sendData)
            .done(function (data) {
                if (data.result == "true") {
                    $('#editNumberForm').modal("hide");
                    document.location = document.location;
                } else {
                    ServerErrorException(data, serverAlert);
                }
            })
            .fail(function (data) {
                ServerErrorException(data, serverAlert);
            });
    }
})

$('button#saveUser').on('click', function () {
    var form = $('#editUserForm').find('.modal-body form')
    var serverAlert = $('#editUserForm').find('.modal-content div#server_alert');

    serverAlert.hide();

    if (form.find('input#user-id').val() == 0) {
        $.post("/index/AjaxNewUser/", form.serialize())
            .done(function (data) {
                if (data.result == "true") {
                    $('#editUserForm').modal("hide");
                    document.location = document.location;
                } else {
                    ServerErrorException(data, serverAlert);
                }
            })
            .fail(function (data) {
                ServerErrorException(data, serverAlert);
            });
    } else {
        $.post("/index/AjaxEditUser/", form.serialize())
            .done(function (data) {
                if (data.result == "true") {
                    $('#editUserForm').modal("hide");
                    document.location = document.location;
                } else {
                    ServerErrorException(data, serverAlert);
                }
            })
            .fail(function (data) {
                ServerErrorException(data, serverAlert);
            });
    }
})

function ServerErrorException(data, alertBox) {
    var errorString = 'Server Error!';

    if (data.status == 403) {
        if (data.responseJSON.errorCode == 1) {
            errorString += ' Entered user information already exists!'
        } else if (data.responseJSON.errorCode == 2) {
            errorString += ' Entered phone number already exists!'
        } if (data.responseJSON.errorCode == 3) {
            errorString += ' Selected user doesn\'t exists!'
        } if (data.responseJSON.errorCode == 4) {
            errorString += ' Incorrect phone number format!'
        } if (data.responseJSON.errorCode == 5) {
            errorString += ' Incorrect user information format!'
        }
    }

    alertBox.text(errorString).show();
}

function PreparePhoneData(sendData) {
    var returnData = sendData;

    for (var key in returnData) {
        if (returnData[key].name == "number") {
            returnData[key].value = OnlyDigit(returnData[key].value);
        }
    }

    return returnData;
}

function OnlyDigit(inStr) {
    var outStr = "";

    for (var i = 0; i < inStr.length; i++) {
        if (IsDigit(inStr[i])) {
            outStr += inStr[i];
        }
    }

    return outStr;
}

function IsDigit(c) {
    return (c >= '0' && c <= '9');
}

function ClearAlerts() {
    $('div.alert').hide();
}

$('div.modal').on('show.bs.modal', function (event) {
    ClearAlerts();
});

$(document).ready(function () {
    $('#phone-number').mask('+0(000)000-00-00');

    var defaultValidator = {
        errorElement: "div",
        errorClass: 'alert',

        errorPlacement: function (error, element) {
            error.addClass('alert-warning').insertAfter(element);
        },
        highlight: function (element) {
            $(element).next().removeClass('alert-success').addClass('alert-warning');
        },
        unhighlight: function (element) {
            $(element).next().removeClass('alert-warning').addClass('alert-success');
        },
        success: function (label) {
            label.text('Ok!');
        }
    };

    var numberValidator = Object.assign(
        {
            rules: {
                number: {
                    minlength: 16,
                    maxlength: 16,
                    required: true
                }
            },
            messages: {
                number: 'Phone format: +X(XXX)XXX-XX-XX!'
            }
        }, defaultValidator
    );
    var userValidator = Object.assign(
        {
            rules: {
                uname: {
                    minlength: 3,
                    maxlength: 32,
                    required: true
                },
                sname: {
                    minlength: 3,
                    maxlength: 32,
                    required: true
                },
                fname: {
                    minlength: 3,
                    maxlength: 32,
                    required: true
                },
                address: {
                    minlength: 2,
                    maxlength: 256,
                    required: true
                },
            }
        }, defaultValidator
    );

    $('#editNumberForm').find('.modal-body form').validate(numberValidator);
    $('#editUserForm').find('.modal-body form').validate(userValidator);
});

function PhoneMask(number) {
    var maskedPhone = "+" + number[0] + "(";

    for (var i = 1; i < number.length; i++) {
        maskedPhone += number[i];

        if (i == 3) {
            maskedPhone += ")";
        } else if (i == 6 || i == 8) {
            maskedPhone += "-";
        }
    }

    return maskedPhone; 
}