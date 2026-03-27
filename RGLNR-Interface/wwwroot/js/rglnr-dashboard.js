// ===== Permissions Modal =====
document.addEventListener("DOMContentLoaded", function () {
    const modal = document.getElementById('userPermissionsModal');
    const modalContent = modal.querySelector('.relative.p-4');

    function showModal() {
        modal.classList.remove('hidden');
        setTimeout(() => {
            modal.classList.add('bg-opacity-50');
            modalContent.classList.add('scale-100', 'opacity-100');
            modalContent.classList.remove('scale-95', 'opacity-0');
        }, 10);
    }

    function hideModal() {
        modal.classList.remove('bg-opacity-50');
        modalContent.classList.remove('scale-100', 'opacity-100');
        modalContent.classList.add('scale-95', 'opacity-0');
        setTimeout(() => {
            modal.classList.add('hidden');
        }, 300);
    }

    document.querySelectorAll('[data-modal-toggle="userPermissionsModal"]').forEach(button => {
        button.addEventListener('click', showModal);
    });

    document.querySelectorAll('[data-modal-hide="userPermissionsModal"]').forEach(button => {
        button.addEventListener('click', hideModal);
    });
});

// ===== Filter Tab Navigation =====
document.addEventListener("DOMContentLoaded", function () {
    const tabs = document.querySelectorAll('.tab');
    let activeTab = tabs[1];
    const filterDiv = document.getElementById(activeTab.getAttribute('data-filter'));

    activeTab.classList.add('border-blue-600', 'text-blue-600');
    filterDiv.style.display = 'block';

    tabs.forEach(tab => {
        tab.addEventListener('click', function () {
            const filterId = this.getAttribute('data-filter');
            const filterDiv = document.getElementById(filterId);

            if (activeTab !== this) {
                const activeFilter = document.getElementById(activeTab.getAttribute('data-filter'));
                activeFilter.style.display = 'none';
                activeTab.classList.remove('border-blue-600', 'text-blue-600');

                filterDiv.style.display = 'block';
                this.classList.add('border-blue-600', 'text-blue-600');
                activeTab = this;
            }
        });
    });
});

// ===== Dropdown & Company Filter =====
function closeDropdown() {
    document.getElementById('dropdownNavbar').classList.add('hidden');
}

document.querySelectorAll('#dropdownNavbar a').forEach(item => {
    item.addEventListener('click', () => {
        closeDropdown();
    });
});

document.getElementById('sendEmailButton').addEventListener('click', function () {
    document.getElementById('alert-border-1').classList.remove('hidden');
});

document.querySelector('[data-dismiss-target="#alert-border-1"]').addEventListener('click', function () {
    document.getElementById('alert-border-1').classList.add('hidden');
});

var currentCompanyFilter = '-1';

document.querySelectorAll('.dropdown-item').forEach(item => {
    item.addEventListener('click', function () {
        var value = this.getAttribute('data-value');
        currentCompanyFilter = value;

        document.getElementById('navbarDropdown').textContent = this.textContent;

        $('#logRGLNRTable').DataTable().ajax.reload();
    });
});

// ===== Date Range Picker =====
document.addEventListener('DOMContentLoaded', function () {
    const dateRangePickerElement = document.getElementById("date-range-picker");
    const $datePicker1 = document.getElementById("datepicker-range-start");
    const $datePicker2 = document.getElementById("datepicker-range-end");

    const dateRangePicker = new DateRangePicker(dateRangePickerElement, {
        inputs: [$datePicker1, $datePicker2],
        language: 'de',
        format: 'dd.mm.yyyy',
        clearBtn: true,
        todayBtn: true,
        todayBtnMode: 1,
    });
});

// ===== Search Form HTML =====
var searchFormHtml = `
<form id="generalsearchform" class="max-w-lg mx-auto">
    <div class="relative flex">
        <label for="search-dropdown" class="mb-2 text-sm font-medium text-gray-900 sr-only dark:text-white">Search</label>
        <button id="dropdown-button" class="flex-shrink-0 z-10 inline-flex items-center py-2 px-3 text-sm font-medium text-center text-gray-900 bg-gray-100 border border-gray-300 rounded-l-lg hover:bg-gray-200 focus:ring-4 focus:outline-none focus:ring-gray-100 dark:bg-gray-700 dark:hover:bg-gray-600 dark:focus:ring-gray-700 dark:text-white dark:border-gray-600" type="button">
            Alle Kategorien
            <svg class="w-2 h-2 ml-2" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 10 6">
                <path stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M1 1l4 4 4-4"/>
            </svg>
        </button>
        <div id="dropdown" class="absolute top-full mt-2 left-0 z-10 hidden bg-white divide-y divide-gray-100 rounded-lg shadow w-44 dark:bg-gray-700">
            <ul class="py-2 text-sm text-gray-700 dark:text-gray-200" aria-labelledby="dropdown-button">
                <li><button type="button" data-field="searchallcategories" class="dropdown-search-item inline-flex w-full px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-600 dark:hover:text-white">Alle Kategorien</button></li>
                <hr class="my-2 border-gray-200 dark:border-gray-600" />
                <li><button type="button" data-field="searchziel" class="dropdown-search-item inline-flex w-full px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-600 dark:hover:text-white">Ziel</button></li>
                <li><button type="button" data-field="searchdebitor" class="dropdown-search-item inline-flex w-full px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-600 dark:hover:text-white">Debitorenkonto</button></li>
                <li><button type="button" data-field="searchrglnr" class="dropdown-search-item inline-flex w-full px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-600 dark:hover:text-white">Rechnungsliste</button></li>
                <li><button type="button" data-field="searchinvoice" class="dropdown-search-item inline-flex w-full px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-600 dark:hover:text-white">Rechnungsnummer</button></li>
                <li><button type="button" data-field="searchdebitorrequest" class="dropdown-search-item inline-flex w-full px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-600 dark:hover:text-white">Anforderung Debitor</button></li>
                <li><button type="button" data-field="searchdebitorreference" class="dropdown-search-item inline-flex w-full px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-600 dark:hover:text-white">Debitorenreferenz</button></li>
                <li><button type="button" data-field="searchcreatedby" class="dropdown-search-item inline-flex w-full px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-600 dark:hover:text-white">Erstellt Von</button></li>
                <li><button type="button" data-field="searchjobnr" class="dropdown-search-item inline-flex w-full px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-600 dark:hover:text-white">Job Nummer</button></li>
                <li><button type="button" data-field="searchlobsterprofile" class="dropdown-search-item inline-flex w-full px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-600 dark:hover:text-white">Lobster Profil</button></li>
                <li><button type="button" data-field="searchlobsterstatus" class="dropdown-search-item inline-flex w-full px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-600 dark:hover:text-white">Lobster Status</button></li>
            </ul>
        </div>
        <div class="relative w-full">
            <input type="search" id="dynamicInput" class="block p-2 w-full z-20 text-sm text-gray-900 bg-gray-50 rounded-r-lg border-l-gray-50 border-l-2 border border-gray-300 focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:border-l-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:border-blue-500" title="Suchen oder 'NONE' f\u00fcr leere Werte eingeben" placeholder="Suchen..." autocomplete="off"/>
            <button id="activateSearch" type="submit" class="absolute top-0 right-0 p-2 text-sm font-medium h-full text-white bg-blue-700 rounded-r-lg border border-blue-700 hover:bg-blue-800 focus:ring-4 focus:outline-none focus:ring-blue-300 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800">
                <svg class="w-4 h-4" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 20 20">
                    <path stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 19l-4-4M15 8a7 7 0 1 1-14 0 7 7 0 0 1 14 0z"/>
                </svg>
                <span class="sr-only">Suche</span>
            </button>
        </div>
    </div>
</form>`;

// ===== DataTables Initialization =====
var selectedField = 'searchallcategories';

function debounce(callback, delay) {
    let timeoutId;
    return function () {
        const context = this;
        const args = arguments;

        clearTimeout(timeoutId);
        timeoutId = setTimeout(function () {
            callback.apply(context, args);
        }, delay);
    };
}

moment.locale('de');

$(document).ready(function () {
    var config = window.RGLNR_CONFIG;

    var table = $('#logRGLNRTable').DataTable({
        "processing": true,
        "serverSide": true,
        "colReorder": true,
        "ordering": true,
        "autoWidth": false,
        "dom": '<"flex justify-between items-center p-2"' +
            '<"my-custom-element p-2">' +
            '<"l-built-in-elements flex items-center space-x-2 hidden"l>' +
            '<"dt-built-in-elements hidden flex items-center space-x-6 ml-auto"' +
            '<"flex-grow">' +
            '<"dt-buttons flex items-center space-x-2 whitespace-nowrap"B>' +
            '>' +
            '>' +
            'rt' +
            '<"flex justify-between items-center p-2"<i><p>>',

        "initComplete": function () {
            var dataTableApi = this.api();

            $('#logRGLNRTable_length label').removeClass().addClass('text-sm text-gray-600');
            $('#logRGLNRTable_length select').removeClass().addClass('text-sm px-3 py-1 border border-gray-300 rounded-lg bg-white');
            $('#logRGLNRTable_paginate').addClass('text-sm space-x-2 flex items-center');
            $('#logRGLNRTable_paginate .paginate_button').removeClass().addClass('text-sm px-2 py-1 bg-white border border-gray-300 rounded-lg hover:bg-gray-100');
            $('#logRGLNRTable_info').addClass('text-sm text-gray-600');
            $('.dt-built-in-elements').removeClass('hidden');
            $('.l-built-in-elements').removeClass('hidden');

            $('.my-custom-element').append(searchFormHtml);

            $('.dropdown-search-item').on('click', function (event) {
                event.preventDefault();
                selectedField = $(this).data('field');

                var selectedCategory = $(this).text();

                $('#dropdown-button').html(selectedCategory +
                    '<svg class="w-2 h-2 ml-2" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 10 6">' +
                    '<path stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M1 1l4 4 4-4"/></svg>');

                $('#dropdown').addClass('hidden');
            });

            document.getElementById("generalsearchform").addEventListener("submit", function (event) {
                event.preventDefault();
                var dynamicInputValue = $("#dynamicInput").val();
                if (dynamicInputValue === '') {
                    selectedField = null;
                    $('#dropdown-button').html('Alle Kategorien' +
                        '<svg class="w-2 h-2 ml-2" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 10 6">' +
                        '<path stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M1 1l4 4 4-4"/></svg>');
                }

                table.draw();
            });

            $('#dropdown-button').on('click', function (event) {
                event.preventDefault();
                $('#dropdown').toggleClass('hidden');
            });

            $(document).on('click', function (event) {
                if (!$(event.target).closest('#dropdown-button, #dropdown').length) {
                    $('#dropdown').addClass('hidden');
                }
            });

            dataTableApi.columns.adjust();
        },
        "language": {
            "url": "https://cdn.datatables.net/plug-ins/9dcbecd42ad/i18n/German.json",
            "processing": "<div class='custom-processing flex items-center justify-center'>" +
                "<div class='spinner animate-spin rounded-full h-16 w-16 border-t-4 border-b-4 border-blue-500'></div>" +
                "<span class='ml-4 text-xl text-gray-600'>Daten werden geladen...</span>" +
                "</div>",
            "info": "_START_ bis _END_ von _TOTAL_ Einträgen",
            "infoFiltered": "(gefiltert von _MAX_ Einträgen)"
        },
        buttons: [
            {
                extend: 'collection',
                text: '<span class="flex items-center"><i class="mr-1 mt-0.5 fa-solid fa-table-columns h-4 w-4"></i>Spalten anpassen<svg class="ml-1 h-4 w-4" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="none" viewBox="0 0 24 24"><path stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="m19 9-7 7-7-7" /></svg></span>',
                className: 'inline-flex items-center justify-center rounded-lg border border-gray-200 bg-white px-3 py-2 text-sm font-medium text-gray-900 hover:bg-gray-100 hover:text-primary-700 focus:z-10 focus:outline-none focus:ring-4 focus:ring-gray-100 dark:border-gray-600 dark:bg-gray-800 dark:text-gray-400 dark:hover:bg-gray-700 dark:hover:text-white dark:focus:ring-gray-700',
                buttons: function (dt) {
                    var buttons = [];
                    dt.columns().every(function (idx) {
                        var column = this;
                        var title = $(column.header()).text();
                        buttons.push({
                            text: function () {
                                var checked = column.visible() ? 'checked' : '';
                                return '<label class="flex items-center">' +
                                    '<input type="checkbox" ' + checked + ' class="mr-2 dt-checkbox text-blue-600 bg-gray-100 border-gray-300 rounded focus:ring-blue-500 dark:focus:ring-blue-600 dark:ring-offset-gray-800 focus:ring-2 dark:bg-gray-700 dark:border-gray-600" ' +
                                    'data-column-index="' + idx + '" onclick="event.stopPropagation();">' +
                                    title + '</label>';
                            },
                            action: function () {
                            },
                            className: 'dt-button'
                        });
                    });
                    return buttons;
                },
                init: function (dt, node, config) {
                    $(document).on('change', 'input.dt-checkbox', function () {
                        var colIdx = $(this).data('column-index');
                        var vis = $(this).prop('checked');
                        dt.column(colIdx).visible(vis, false);
                        dt.columns.adjust().draw(false);
                    });
                }
            },
            {
                extend: 'collection',
                text: '<span class="flex items-center"><i class="mr-1 fas fa-download h-4 w-4"></i>Exportieren <svg class="ml-1 h-4 w-4" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="none" viewBox="0 0 24 24"><path stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="m19 9-7 7-7-7" /></svg></span>',
                className: 'inline-flex items-center justify-center rounded-lg text-white bg-blue-700 px-3 py-2 text-sm font-medium hover:bg-blue-800 focus:ring-4 focus:outline-none focus:ring-blue-300 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800',
                buttons: [
                    { extend: 'copy', exportOptions: { columns: ':visible' }, text: '<i class="mr-1 fas fa-copy h-4 w-4"></i> Kopieren', className: 'dt-button-option' },
                    { extend: 'csv', exportOptions: { columns: ':visible' }, text: '<i class="mr-1 fas fa-file-csv h-4 w-4"></i> CSV', className: 'dt-button-option' },
                    { extend: 'excel', exportOptions: { columns: ':visible' }, text: '<i class="mr-1 fas fa-file-excel h-4 w-4"></i> Excel', className: 'dt-button-option' },
                    { extend: 'pdf', exportOptions: { columns: ':visible' }, orientation: 'landscape', pageSize: 'A3', text: '<i class="mr-1 fas fa-file-pdf h-4 w-4"></i> PDF', className: 'dt-button-option' },
                    { extend: 'print', exportOptions: { columns: ':visible' }, text: '<i class="mr-1 fas fa-print h-4 w-4"></i> Drucken', className: 'dt-button-option' }
                ]
            }
        ],
        "iDisplayLength": 15,
        "lengthMenu": [[5, 10, 15, 20, 25, 50, 100, 200, 100000], [5, 10, 15, 20, 25, 50, 100, 200, "Alle"]],

        "ajax": {
            "url": config.loadDataUrl,
            "type": "POST",
            "data": function (d) {
                d.minRGLNR = $("#minRGLNR").val();
                d.maxRGLNR = $("#maxRGLNR").val();
                d.pasteInvoices = $("#pasteInvoices").val();
                d.companyPrefix = currentCompanyFilter;
                d.searchField = selectedField;
                var startDateStr = document.getElementById('datepicker-range-start').value;
                var endDateStr = document.getElementById('datepicker-range-end').value;
                var dynamicInputValue = $("#dynamicInput").val();
                d.searchValue = dynamicInputValue === '' ? null : dynamicInputValue;
                if (startDateStr && endDateStr) {
                    var selectedColumn = document.getElementById('dateColumnFilter').value;

                    function formatDate(inputDate) {
                        var parts = inputDate.split('.');
                        return parts[2] + '-' + parts[1] + '-' + parts[0];
                    }

                    var formattedStartDate = formatDate(startDateStr);
                    var formattedEndDate = formatDate(endDateStr);

                    if (selectedColumn === 'datum') {
                        d.startDate = formattedStartDate;
                        d.endDate = formattedEndDate;
                        d.faelligStart = null; d.faelligEnd = null;
                        d.bestaetigungStart = null; d.bestaetigungEnd = null;
                    } else if (selectedColumn === 'faellig') {
                        d.faelligStart = formattedStartDate;
                        d.faelligEnd = formattedEndDate;
                        d.startDate = null; d.endDate = null;
                        d.bestaetigungStart = null; d.bestaetigungEnd = null;
                    } else if (selectedColumn === 'bestaetigung') {
                        d.bestaetigungStart = formattedStartDate;
                        d.bestaetigungEnd = formattedEndDate;
                        d.startDate = null; d.endDate = null;
                        d.faelligStart = null; d.faelligEnd = null;
                    } else {
                        d.startDate = null; d.endDate = null;
                        d.faelligStart = null; d.faelligEnd = null;
                        d.bestaetigungStart = null; d.bestaetigungEnd = null;
                    }
                } else {
                    d.startDate = null; d.endDate = null;
                    d.faelligStart = null; d.faelligEnd = null;
                    d.bestaetigungStart = null; d.bestaetigungEnd = null;
                }
            }
        },
        "columns": [
            {
                "name": "flow",
                "data": null,
                "render": function (data, type, row) {
                    const destinationBadges = [];
                    const destinationKeys = new Set();
                    const source = data.source;

                    if (source) {
                        const normalizedSource = source.toUpperCase();
                        const sourceBadgeClass = 'inline-flex items-center rounded-md border px-2 py-1 text-xs font-semibold tracking-wide';
                        const sourceBadgeStyle = normalizedSource === 'D365'
                            ? 'background-color:#F5F3FF;border-color:#DDD6FE;color:#6D28D9;'
                            : 'background-color:#EFF6FF;border-color:#BFDBFE;color:#1D4ED8;';
                        var sourceBadge = `<span class="${sourceBadgeClass}" style="${sourceBadgeStyle}">${normalizedSource}</span>`;
                    } else {
                        var sourceBadge = '<span class="inline-flex items-center rounded-md border px-2 py-1 text-xs font-semibold tracking-wide" style="background-color:#F9FAFB;border-color:#E5E7EB;color:#6B7280;">N/A</span>';
                    }

                    function buildDestinationBadge(label) {
                        const normalizedLabel = (label || '').toLowerCase();
                        let style = 'background-color:#374151;color:#FFFFFF;';
                        if (normalizedLabel === 'printer') { style = 'background-color:#2563EB;color:#FFFFFF;'; }
                        else if (normalizedLabel === 'email') { style = 'background-color:#059669;color:#FFFFFF;'; }
                        else if (normalizedLabel === 'customport') { style = 'background-color:#EA580C;color:#FFFFFF;'; }
                        else if (normalizedLabel === 'edi') { style = 'background-color:#7C3AED;color:#FFFFFF;'; }
                        else if (normalizedLabel === 'unbekannt') { style = 'background-color:#9CA3AF;color:#FFFFFF;'; }
                        return `<span class="inline-flex items-center rounded-md px-2 py-1 text-xs font-semibold tracking-wide" style="${style}">${label}</span>`;
                    }

                    function addDestinationBadge(label) {
                        const key = (label || '').trim().toLowerCase();
                        if (!key || destinationKeys.has(key)) { return; }
                        destinationKeys.add(key);
                        destinationBadges.push(buildDestinationBadge(label));
                    }

                    const method = data.method ? data.method.toLowerCase() : '';
                    if (method === 'edi') {
                        addDestinationBadge('EDI');
                    } else if (method === 'lasernet' && data.destinationType) {
                        addDestinationBadge(data.destinationType);
                    } else if (data.destinationType) {
                        addDestinationBadge(data.destinationType);
                    }
                    if (data.profile_name) { addDestinationBadge('EDI'); }
                    if (!destinationBadges.length) { addDestinationBadge('Unbekannt'); }

                    return `
                        <div class="flex items-center gap-1 whitespace-nowrap">
                            ${sourceBadge}
                            <span class="px-1 text-sm font-semibold text-slate-400">&rarr;</span>
                            ${destinationBadges.join('<span class="px-0.5 text-xs text-slate-300">+</span>')}
                        </div>`;
                }
            },
            { "name": "debitorenkonto", "data": "debitorenkonto" },
            { "name": "rglnr", "data": "rglnr" },
            { "name": "rechnung", "data": "rechnung" },
            { "name": "datum", "data": "datum", "render": function (data) { return moment(data).format("DD.MM.YYYY"); } },
            { "name": "faellig", "data": "f\u00e4llig", "visible": false, "render": function (data) { return moment(data).format("DD.MM.YYYY"); } },
            {
                "name": "entry_date",
                "data": "entry_date", "render": function (data) {
                    if (!moment(data).isValid()) { return ''; }
                    else { return moment(data).format("DD.MM.YYYY"); }
                }
            },
            { "name": "rechnungsbetrag", "data": "rechnungsbetrag" },
            { "name": "materialanforderung", "data": "materialanforderung", "visible": false },
            { "name": "ihrzeichen", "data": "ihrzeichen" },
            { "name": "createdby", "data": "createdby" },
            {
                "name": "createddatetime",
                "data": "createddatetime", "render": function (data) {
                    if (!moment(data).isValid()) { return ''; }
                    else { return moment(data).format("DD.MM.YYYY HH:mm"); }
                }
            },
            { "name": "job_nr", "data": "job_nr", "visible": false },
            { "name": "profile_name", "data": "profile_name", "visible": false },
            { "name": "status", "data": "status", "visible": false },
        ],
        "order": [[4, "desc"]],
        "deferRender": true,
        "stripeClasses": [],
        "rowCallback": function (row, data, index) {
            $(row).addClass('bg-white border-b dark:bg-gray-800 dark:border-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600');
            $('td', row).addClass('px-4 py-2 text-gray-500 dark:text-gray-400');
        }
    });

    // ===== Filter Event Handlers =====
    function handleRGLNRChange() {
        var minRGLNR = $('#minRGLNR').val();
        var maxRGLNR = $('#maxRGLNR').val();
        var pasteInvoices = $('#pasteInvoices').val();
        table.draw();
    }

    var debouncedRGLNRChange = debounce(handleRGLNRChange, 300);

    $('#minRGLNR').on('input', debouncedRGLNRChange);
    $('#maxRGLNR').on('input', debouncedRGLNRChange);
    $('#pasteInvoices').on('input', handleRGLNRChange);

    document.getElementById('dateColumnFilter').addEventListener('change', function () {
        if (document.getElementById('datepicker-range-start').value || document.getElementById('datepicker-range-end').value) {
            table.draw();
        }
    });

    $('#datepicker-range-start, #datepicker-range-end').on('changeDate', function (ev, picker) {
        console.log('Zeitraum angewendett');
        table.draw();
    });

    table.on('buttons-popover.dt', function () {
        $('.dt-button-collection .dt-button').addClass('dt-button-option');
    });

    $('#logRGLNRTable_paginate').addClass('text-sm space-x-2 flex items-center');
    $('#logRGLNRTable_paginate .paginate_button').removeClass().addClass('text-sm px-3 py-1 bg-white border border-gray-300 rounded-lg hover:bg-gray-100');
    $('#logRGLNRTable_info').addClass('text-sm text-gray-600');
    $('#logRGLNRTable_length select').removeClass().addClass('text-sm px-3 py-1 border border-gray-300 rounded-lg bg-white');

    $('#companyFilter').on('change', function (e) {
        table.draw();
    });

    // ===== Row Double-Click Detail Modal =====
    $('#logRGLNRTable tbody').on('dblclick', 'tr', function () {
        var data = table.row(this).data();
        if (data) {
            var invoiceId = data.rechnung;
            var entry_date = moment(data.entry_date).format('YYYY-MM-DD, HH:mm:ss');
            $.ajax({
                "url": config.getInvoiceDetailsUrl,
                "type": "POST",
                "data": { invoiceId, entry_date },
                "success": function (response) {
                    showModalWithData(response);
                },
                "error": function (xhr, status, error) {
                    console.error('Fehler: ', error);
                }
            });
        }
    });

    // ===== Detail Modal Renderer =====
    function showModalWithData(dataArray) {
        $('#modalTitle').text('Details zu Rechnung: ' + dataArray[0].rechnung);

        var tabsHeader = `
            <ul class="flex flex-wrap -mb-px text-sm font-medium text-center text-gray-500 border-b border-gray-200 dark:text-gray-400 dark:border-gray-700" id="tabs" data-tabs-toggle="#tabs-content" role="tablist">
            `;
        var tabsContent = '<div id="tabs-content">';

        dataArray.forEach(function (data, index) {
            var tabId = 'tab-' + index;
            var contentId = 'tab-content-' + index;

            var tabButtonClass = index === 0
                ? 'inline-block p-4 text-blue-600 border-b-2 border-blue-600 rounded-t-lg active'
                : 'inline-block p-4 border-b-2 border-transparent rounded-t-lg hover:text-gray-600 hover:border-gray-300';

            tabsHeader += `
                <li class="mr-2" role="presentation">
                    <button class="${tabButtonClass}" id="${tabId}" data-tabs-target="#${contentId}" type="button" role="tab" aria-controls="${contentId}" aria-selected="${index === 0 ? 'true' : 'false'}">
                        Eintrag ${index + 1} (${moment(data.datum).format('DD.MM.YYYY')})
                    </button>
                </li>`;

            var contentClass = index === 0
                ? 'p-4 bg-gray-50 rounded-lg dark:bg-gray-800'
                : 'hidden p-4 bg-gray-50 rounded-lg dark:bg-gray-800';

            tabsContent += `
            <div class="${contentClass} p-4" id="${contentId}" role="tabpanel" aria-labelledby="${tabId}">
                <div class="border-b-2 pb-4 mb-4">
                    <p><strong>Rechnungsliste:</strong> ${data.rglnr ? data.rglnr : '<span class="text-gray-500 italic">Nicht verf\u00fcgbar</span>'}</p>
                    <p><strong>Debitorenkonto:</strong> ${data.debitorenkonto ? data.debitorenkonto : '<span class="text-gray-500 italic">Nicht verf\u00fcgbar</span>'}</p>
                    <p><strong>HOS:</strong> ${data.hos ? data.hos : '<span class="text-gray-500 italic">Nicht verf\u00fcgbar</span>'}</p>
                    <p><strong>Erstellungsdatum:</strong> ${data.createddatetime ? moment(data.createddatetime).format('DD.MM.YYYY HH:mm.ss') : '<span class="text-gray-500 italic">Erstellungsdatum nicht verf\u00fcgbar</span>'}</p>
                    <p><strong>Buchung:</strong> ${data.datum ? moment(data.datum).format('DD.MM.YYYY') : '<span class="text-gray-500 italic">Kein Datum verf\u00fcgbar</span>'}</p>
                    <p><strong>F\u00e4llig:</strong> ${data.f\u00e4llig ? moment(data.f\u00e4llig).format('DD.MM.YYYY') : '<span class="text-gray-500 italic">F\u00e4lligkeitsdatum nicht festgelegt</span>'}</p>
                    <p><strong>Best\u00e4tigung:</strong> ${data.entry_date ? moment(data.entry_date).format('DD.MM.YYYY HH:mm') : '<span class="text-gray-500 italic">Best\u00e4tigung nicht verf\u00fcgbar</span>'}</p>
                    <p><strong>Betrag:</strong> ${data.rechnungsbetrag ? data.rechnungsbetrag : '<span class="text-gray-500 italic">Betrag nicht angegeben</span>'}</p>
                    <p><strong>Anforderung Debitor:</strong> ${data.materialanforderung ? data.materialanforderung : '<span class="text-gray-500 italic">Nicht verf\u00fcgbar</span>'}</p>
                    <p><strong>Debitorenreferenz:</strong> ${data.ihrzeichen ? data.ihrzeichen : '<span class="text-gray-500 italic">Nicht verf\u00fcgbar</span>'}</p>
                    <p><strong>Job Nummer:</strong> ${data.job_nr ? data.job_nr : '<span class="text-gray-500 italic">Keine Jobnummer</span>'}</p>
                    <p><strong>Lobster Profil:</strong> ${data.profile_name ? data.profile_name : '<span class="text-gray-500 italic">Profil nicht verf\u00fcgbar</span>'}</p>
                    <p><strong>Lobster Status:</strong> ${data.status ? data.status : '<span class="text-gray-500 italic">Status nicht verf\u00fcgbar</span>'}</p>
                </div>
                <div>
                    <h2 class="text-lg font-semibold text-gray-800 mb-2">Lasernet</h2>
                    <p><strong>Drucker:</strong> ${data.printer ? data.printer : '<span class="text-gray-500 italic">Kein Drucker verf\u00fcgbar</span>'}</p>
                    <p><strong>Email Von:</strong> ${data.emailfrom ? data.emailfrom : '<span class="text-gray-500 italic">Nicht verf\u00fcgbar</span>'}</p>
                    <p><strong>Email Zu:</strong> ${data.emailto ? data.emailto : '<span class="text-gray-500 italic">Nicht verf\u00fcgbar</span>'}</p>
                    <p><strong>Port:</strong> ${data.customport ? data.customport : '<span class="text-gray-500 italic">Kein Port verf\u00fcgbar</span>'}</p>
                </div>
            </div>
            `;
        });

        tabsHeader += '</ul>';
        tabsContent += '</div>';

        var modalContent = tabsHeader + tabsContent;
        $('#modalBody').html(modalContent);

        initializeTabs();

        var modalElement = document.getElementById('rowDetailsModal');
        var modal = new Modal(modalElement);
        modal.show();

        document.querySelectorAll('[data-modal-hide="rowDetailsModal"]').forEach((button) => {
            button.addEventListener('click', function () {
                modal.hide();
            });
        });
    }

    // ===== Detail Modal Tab Switching =====
    function initializeTabs() {
        const tabs = document.querySelectorAll('#tabs button');
        let activeTab = tabs[0];

        document.querySelector(activeTab.getAttribute('data-tabs-target')).classList.remove('hidden');

        tabs.forEach(tab => {
            tab.addEventListener('click', function () {
                document.querySelector(activeTab.getAttribute('data-tabs-target')).classList.add('hidden');
                activeTab.classList.remove('text-blue-600', 'border-blue-600');

                const newContent = document.querySelector(this.getAttribute('data-tabs-target'));
                newContent.classList.remove('hidden');
                this.classList.add('text-blue-600', 'border-blue-600');

                activeTab = this;
            });
        });
    }
});
