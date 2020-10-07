"use strict";

// signalr implementation
var connection = new signalR.HubConnectionBuilder().withUrl("/contactListHub").build();
connection.on("RefreshContactList", function () {
    getContactList($('#inputSearch').val());
});
connection.start().catch(function (err) {
    return console.error(err.toString());
});

// search input listener
$('#inputSearch').keyup(function () {
    getContactList($('#inputSearch').val())
});

// track currently selected row
let currentlySelectedRow = null;
function highlightRow(e) {
    if (currentlySelectedRow == e) {
        currentlySelectedRow.style.background = 'white';
        currentlySelectedRow = null;
        return;
    }

    if (currentlySelectedRow != null)
        currentlySelectedRow.style.background = 'white';

    currentlySelectedRow = e;
    currentlySelectedRow.style.background = 'bisque';
}

// get contact list from server based on search term, if any
function getContactList(searchTerm = "") {
    fetch("/Home/GetContacts?searchTerm=" + searchTerm, {
        method: 'POST',
        cache: 'no-cache',
        credentials: 'same-origin',
        referrerPolicy: 'no-referrer'
    }).then(response => response.text())
        .then(body => {
            document.getElementById("section_contactList").innerHTML = body;
        });
}

// get selected contact to edit froms server
function editContact(id) {
    fetch("/Home/EditContact?id=" + id, {
        method: 'POST',
        cache: 'no-cache',
        credentials: 'same-origin',
        referrerPolicy: 'no-referrer'
    }).then(response => response.text())
        .then(body => {
            document.getElementById("modal_createEditContact").innerHTML = body;
            $('#createEditContactModal').modal('show');
        });
}

// post contact edits to server
function submitContact() {
    var xhr = new XMLHttpRequest();
    xhr.open("POST", "/Home/EditCreateContact");
    var formData = new FormData(document.querySelector("#form-editContact"));
    xhr.send(formData);
    xhr.onload = function (event) {
        if (xhr.response == "true") {
            Swal.fire({
                title: 'Saved Successfully',
                confirmButtonText: `Awesome!`,
                allowOutsideClick: false
            }).then((result) => {
                closeNewContact();
                getContactList();
                connection.invoke("DataUpdate").catch(function (err) {
                    console.error(err.toString());
                });;
            })
        } else {
            Swal.fire('Failed to save.', xhr.response, 'warning')
        }
    };
}

// deletes an email partial from the contact
function deleteEmail(index) {
    let isDeletedInput = document.querySelector('#EmailAddresses_' + index + '__IsDeleted');
    if (isDeletedInput !== null)
        isDeletedInput.value = "True";

    document.querySelector("#emailInput-" + index).classList.add("hidden");
}

// close edit/new contact modal
function closeNewContact() {
    $('#createEditContactModal').modal('hide');
    let newContactForm = document.getElementById("modal_createEditContact");
    newContactForm.innerHTML = "";
}

// create a new email address entry and set index according to last available
function newEmailAddress() {
    let newTemplate = $('#emailEditor').clone();

    // determine next index
    let index = 0;
    if (document.getElementById('section-emailAddresses').getElementsByClassName('emailindex').length > 0)
        index = parseInt($('#section-emailAddresses .email').last().find('.emailindex').val()) + 1;

    newTemplate.html($(newTemplate).html().replace(/#/g, index));
    $('#section-emailAddresses').append(newTemplate.html());

    // refresh validator fields
    let form = $('form');
    form.data('validator', null);
    $.validator.unobtrusive.parse(form);
};

function importContacts() {
    $('#importContactsModal').modal('show');
}

function uploadImportFile() {
    var xhr = new XMLHttpRequest();
    xhr.open("POST", "/Home/ImportFile");
    var formData = new FormData(document.getElementById("form-importFile"));
    xhr.send(formData);
    xhr.onload = function () {
        if (xhr.response == "true") {
            Swal.fire({
                title: 'Imported Successfully',
                confirmButtonText: `Awesome!`,
                allowOutsideClick: true
            }).then(() => {
                $('#importContactsModal').modal('hide');
                getContactList();
                connection.invoke("DataUpdate").catch(function (err) {
                    console.error(err.toString());
                });;
            })
        } else {
            Swal.fire('Failed to save.', xhr.response, 'warning')
            $('#importContactsModal').modal('hide');
            $('#input-importFile').val(null);
        }
    };
}