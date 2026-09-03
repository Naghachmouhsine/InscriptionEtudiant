// filiere.js - handles search, delete confirmation and simple client validation for filiere forms
document.addEventListener('DOMContentLoaded', function () {
    var searchInput = document.getElementById('searchInput');
    var searchBtn = document.getElementById('searchBtn');
    var clearBtn = document.getElementById('clearBtn');

    if (searchBtn) {
        searchBtn.addEventListener('click', function () {
            var q = searchInput ? searchInput.value.trim() : '';
            var url = new URL(window.location.href);
            if (q) url.searchParams.set('search', q); else url.searchParams.delete('search');
            window.location.href = url.pathname + url.search;
        });
    }

    if (clearBtn) {
        clearBtn.addEventListener('click', function () {
            if (searchInput) searchInput.value = '';
            var url = new URL(window.location.href);
            url.searchParams.delete('search');
            window.location.href = url.pathname + url.search;
        });
    }
});

function confirmDelete(form) {
    if (!form) return false;
    var ok = confirm('Êtes-vous sûr de vouloir supprimer cette filière ?');
    return ok;
}

function validateFiliereForm(form) {
    // basic HTML5 validation is used; add cross-field validation for dates
    try {
        var debut = form.querySelector('[name="DateDebutInscription"]');
        var fin = form.querySelector('[name="DateFinInscription"]');
        if (debut && fin) {
            var d = new Date(debut.value);
            var f = new Date(fin.value);
            if (isNaN(d.getTime()) || isNaN(f.getTime())) return true;
            if (f <= d) {
                alert('La date de fin doit être strictement supérieure à la date de début.');
                return false;
            }
        }
    } catch (e) {
        // ignore and allow submit; server-side validation will handle errors
    }
    return true;
}
