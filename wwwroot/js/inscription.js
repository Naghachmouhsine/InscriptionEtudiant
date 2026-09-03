document.addEventListener('DOMContentLoaded', () => {
    const root = document.querySelector('.inscription-wizard');
    if (!root) {
        return;
    }

    const messages = window.inscriptionMessages || {};
    const wizardState = normalizeState(window.inscriptionWizardState || {});

    const maxStep = 6;
    let currentStep = clampStep(wizardState.currentStep || 1);

    const stepElements = Array.from(document.querySelectorAll('.wizard-step'));
    const stepIndicators = Array.from(document.querySelectorAll('.progress-steps .step'));
    const nextBtn = document.getElementById('nextBtn');
    const prevBtn = document.getElementById('prevBtn');
    const alertsHost = document.getElementById('wizardAlerts');
    const parcoursForm = document.getElementById('parcoursForm');
    const choicesForm = document.getElementById('choicesForm');
    const choicesList = document.getElementById('choicesList');
    const clearChoicesBtn = document.getElementById('clearChoicesBtn');
    const choicesCounter = document.getElementById('choicesCounter');
    const filiereCards = Array.from(document.querySelectorAll('.filiere-card'));
    const fileDropZone = document.getElementById('fileDropZone');
    const fileInput = document.getElementById('docsInput');
    const fileList = document.getElementById('fileList');
    const docsCounter = document.getElementById('docsCounter');
    const docsForm = document.getElementById('docsForm');
    const submitForm = document.getElementById('submitForm');
    const submitConfirmModalEl = document.getElementById('submitConfirmModal');
    const confirmSubmitBtn = document.getElementById('confirmSubmitBtn');
    const finalConfirmation = document.getElementById('finalConfirmation');
    const reviewArea = document.getElementById('reviewArea');

    function normalizeState(state) {
        return {
            candidatId: state.candidatId || 0,
            cne: state.cne || '',
            nom: state.nom || '',
            prenom: state.prenom || '',
            email: state.email || '',
            telephone: state.telephone || '',
            currentStep: state.currentStep || 1,
            parcours: state.parcours || {},
            choixFilieres: Array.isArray(state.choixFilieres) ? state.choixFilieres : [],
            documents: Array.isArray(state.documents) ? state.documents : []
        };
    }

    function clampStep(step) {
        return Math.min(Math.max(Number(step || 1), 1), maxStep);
    }

    function escapeHtml(value) {
        return String(value ?? '')
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;');
    }

    function createAlert(type, message, timeout = 0) {
        if (!alertsHost || !message) {
            return null;
        }

        const iconMap = {
            success: 'bi-check-circle-fill',
            danger: 'bi-exclamation-triangle-fill',
            warning: 'bi-exclamation-circle-fill',
            info: 'bi-info-circle-fill'
        };

        const wrapper = document.createElement('div');
        wrapper.innerHTML = `
            <div class="alert alert-${type} alert-dismissible fade show" role="alert" aria-live="polite">
                <span class="d-flex align-items-start">
                    <span class="alert-icon"><i class="bi ${iconMap[type] || iconMap.info}"></i></span>
                    <span>${escapeHtml(message)}</span>
                </span>
                <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Fermer le message"></button>
            </div>
        `;

        const alert = wrapper.firstElementChild;
        alertsHost.appendChild(alert);
        requestAnimationFrame(() => alert.scrollIntoView({ behavior: 'smooth', block: 'start' }));

        if (type === 'success' && timeout > 0) {
            window.setTimeout(() => dismissAlert(alert), timeout);
        }

        return alert;
    }

    function dismissAlert(alert) {
        if (!alert || !alert.isConnected) {
            return;
        }

        alert.classList.add('is-fading');
        window.setTimeout(() => alert.remove(), 200);
    }

    function clearProgressAlerts() {
        if (!alertsHost) {
            return;
        }

        Array.from(alertsHost.querySelectorAll('[data-progress="true"]')).forEach(alert => dismissAlert(alert));
    }

    function showStep(step) {
        currentStep = clampStep(step);
        wizardState.currentStep = currentStep;

        stepElements.forEach(section => {
            section.classList.toggle('d-none', Number(section.dataset.step) !== currentStep);
        });

        stepIndicators.forEach((indicator, index) => {
            indicator.classList.toggle('active', index + 1 === currentStep);
            indicator.classList.toggle('completed', index + 1 < currentStep);
            indicator.setAttribute('aria-current', index + 1 === currentStep ? 'step' : 'false');
        });

        if (nextBtn) {
            nextBtn.textContent = currentStep === 6 ? 'Confirmer mon inscription' : 'Suivant';
        }

        if (currentStep === 5) {
            renderReview();
        }
    }

    function focusFirstInvalidField(form) {
        const invalid = form?.querySelector(':invalid');
        if (invalid) {
            invalid.focus();
        }
    }

    function getSelectText(selectId) {
        const select = document.getElementById(selectId);
        const option = select?.selectedOptions?.[0];
        return option && option.value ? option.textContent.trim() : '';
    }

    function formatDecimal(value) {
        if (value === null || value === undefined || value === '') {
            return '—';
        }

        return String(value);
    }

    function getFiliereName(filiereId) {
        return document.querySelector(`.filiere-card[data-id="${filiereId}"]`)?.dataset.name || '';
    }

    function updateChoicesCounter() {
        if (!choicesList || !choicesCounter) {
            return;
        }

        const count = choicesList.querySelectorAll('li.choice-item').length;
        choicesCounter.textContent = `${count} / 3 filières sélectionnées`;
    }

    function syncChoicesFromList() {
        if (!choicesList) {
            return;
        }

        wizardState.choixFilieres = Array.from(choicesList.querySelectorAll('li.choice-item')).map((item, index) => ({
            ordreChoix: index + 1,
            filiereId: Number(item.dataset.id || 0),
            filiereNom: item.querySelector('.choice-name')?.textContent?.trim() || ''
        }));
        updateChoicesCounter();
    }

    function setCardSelected(filiereId, selected) {
        const card = document.querySelector(`.filiere-card[data-id="${filiereId}"]`);
        if (!card) {
            return;
        }

        card.classList.toggle('selected', selected);
        const button = card.querySelector('.select-btn');
        if (button) {
            button.setAttribute('aria-pressed', selected ? 'true' : 'false');
        }
    }

    function renderChoicesList() {
        if (!choicesList) {
            return;
        }

        const choices = (wizardState.choixFilieres || []).slice().sort((left, right) => left.ordreChoix - right.ordreChoix);
        if (!choices.length) {
            choicesList.innerHTML = `<li class="choice-empty">${messages.infoNoChoicesYet || 'Choisissez jusqu\'à trois filières afin de poursuivre.'}</li>`;
            updateChoicesCounter();
            return;
        }

        choicesList.innerHTML = '';
        choices.forEach((choice, index) => {
            const item = document.createElement('li');
            item.className = 'choice-item list-group-item';
            item.draggable = true;
            item.dataset.id = String(choice.filiereId);
            item.innerHTML = `
                <div>
                    <span class="badge bg-primary me-2 choice-badge">Choix ${index + 1}</span>
                    <strong class="choice-name">${escapeHtml(choice.filiereNom || getFiliereName(choice.filiereId) || 'Filière')}</strong>
                </div>
                <button type="button" class="btn btn-sm btn-link text-danger remove-choice">Retirer</button>
            `;
            choicesList.appendChild(item);
        });

        updateChoicesCounter();
    }

    function addChoice(card) {
        if (!choicesList || !card) {
            return;
        }

        const choiceId = Number(card.dataset.id || 0);
        if (!choiceId) {
            return;
        }

        if (wizardState.choixFilieres.some(choice => choice.filiereId === choiceId)) {
            return;
        }

        if (wizardState.choixFilieres.length >= 3) {
            createAlert('warning', messages.warningMaxChoices || 'Vous pouvez sélectionner jusqu\'à trois filières.', 0);
            return;
        }

        wizardState.choixFilieres.push({
            ordreChoix: wizardState.choixFilieres.length + 1,
            filiereId: choiceId,
            filiereNom: card.dataset.name || ''
        });

        setCardSelected(choiceId, true);
        renderChoicesList();
    }

    function removeChoice(choiceId) {
        wizardState.choixFilieres = wizardState.choixFilieres
            .filter(choice => choice.filiereId !== Number(choiceId))
            .map((choice, index) => ({ ...choice, ordreChoix: index + 1 }));

        setCardSelected(choiceId, false);
        renderChoicesList();
    }

    function hydrateChoicesFromState() {
        document.querySelectorAll('.filiere-card.selected').forEach(card => card.classList.remove('selected'));
        wizardState.choixFilieres.forEach(choice => setCardSelected(choice.filiereId, true));
        renderChoicesList();
    }

    function renderFileList() {
        if (!fileList) {
            return;
        }

        fileList.innerHTML = '';
        const files = Array.from(fileInput?.files || []);

        if (files.length > 0) {
            if (docsCounter) {
                docsCounter.textContent = files.length === 1 ? '1 document' : `${files.length} documents`;
            }

            files.forEach((file, index) => {
                const item = document.createElement('li');
                item.className = 'list-group-item d-flex justify-content-between align-items-center';
                item.innerHTML = `
                    <div>
                        <strong>${escapeHtml(file.name)}</strong>
                        <div class="small text-muted">${(file.size / 1024).toFixed(1)} Ko</div>
                    </div>
                    <button type="button" class="btn btn-sm btn-outline-danger remove-file" data-index="${index}">Retirer</button>
                `;
                fileList.appendChild(item);
            });
            return;
        }

        const documents = wizardState.documents || [];
        if (docsCounter) {
            docsCounter.textContent = documents.length === 1 ? '1 document' : `${documents.length} documents`;
        }

        if (!documents.length) {
            const empty = document.createElement('li');
            empty.className = 'file-empty';
            empty.textContent = messages.infoNoDocumentsYet || 'Aucun document n\'a encore été ajouté.';
            fileList.appendChild(empty);
            return;
        }

        documents.forEach((documentItem, index) => {
            const item = document.createElement('li');
            item.className = 'list-group-item d-flex justify-content-between align-items-center';
            item.innerHTML = `
                <div>
                    <strong>${escapeHtml(documentItem.fileName || `Document ${index + 1}`)}</strong>
                    <div class="small text-muted">${escapeHtml(documentItem.fieldName || 'Pièce jointe')}</div>
                </div>
            `;
            fileList.appendChild(item);
        });
    }

    function renderReview() {
        if (!reviewArea) {
            return;
        }

        const parcours = wizardState.parcours || {};
        const seriesLabel = getSelectText('SerieBacId');
        const mentionLabel = getSelectText('MentionId');
        const choices = wizardState.choixFilieres || [];
        const documents = wizardState.documents || [];

        reviewArea.innerHTML = `
            <div class="review-card card">
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-start gap-3">
                        <div>
                            <span class="step-pill"><i class="bi bi-person-badge"></i> Informations personnelles</span>
                            <h3 class="h5 mt-2 mb-1">${escapeHtml([wizardState.prenom, wizardState.nom].filter(Boolean).join(' '))}</h3>
                            <p class="mb-0 text-muted">${escapeHtml(wizardState.cne || '')}</p>
                        </div>
                        <button type="button" class="btn btn-outline-primary btn-sm" data-step-target="1">Modifier</button>
                    </div>
                    <div class="row g-3">
                        <div class="col-md-6"><strong>Nom</strong><div>${escapeHtml(wizardState.nom || '')}</div></div>
                        <div class="col-md-6"><strong>Prénom</strong><div>${escapeHtml(wizardState.prenom || '')}</div></div>
                        <div class="col-md-6"><strong>CNE</strong><div>${escapeHtml(wizardState.cne || '')}</div></div>
                        <div class="col-md-6"><strong>Email</strong><div>${escapeHtml(wizardState.email || '')}</div></div>
                        <div class="col-md-6"><strong>Téléphone</strong><div>${escapeHtml(wizardState.telephone || '')}</div></div>
                    </div>
                </div>
            </div>
            <div class="review-card card">
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-start gap-3">
                        <div>
                            <span class="step-pill"><i class="bi bi-journal-text"></i> Parcours académique</span>
                            <h3 class="h5 mt-2 mb-1">Résumé du parcours</h3>
                        </div>
                        <button type="button" class="btn btn-outline-primary btn-sm" data-step-target="2">Modifier</button>
                    </div>
                    <div class="row g-3">
                        <div class="col-md-4"><strong>Année du Bac</strong><div>${escapeHtml(parcours.anneeBac || '')}</div></div>
                        <div class="col-md-4"><strong>Série Bac</strong><div>${escapeHtml(seriesLabel || '')}</div></div>
                        <div class="col-md-4"><strong>Mention</strong><div>${escapeHtml(mentionLabel || '')}</div></div>
                        <div class="col-md-6"><strong>Note nationale</strong><div>${escapeHtml(formatDecimal(parcours.noteNationale))}</div></div>
                        <div class="col-md-6"><strong>Note régionale</strong><div>${escapeHtml(formatDecimal(parcours.noteRegionale))}</div></div>
                    </div>
                </div>
            </div>
            <div class="review-card card">
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-start gap-3">
                        <div>
                            <span class="step-pill"><i class="bi bi-list-ol"></i> Choix des filières</span>
                            <h3 class="h5 mt-2 mb-1">Préférences sélectionnées</h3>
                        </div>
                        <button type="button" class="btn btn-outline-primary btn-sm" data-step-target="3">Modifier</button>
                    </div>
                    ${choices.length ? `
                        <ol class="review-list">
                            ${choices.slice().sort((left, right) => left.ordreChoix - right.ordreChoix).map(choice => `
                                <li>${escapeHtml(choice.filiereNom || getFiliereName(choice.filiereId) || 'Filière')}</li>
                            `).join('')}
                        </ol>
                    ` : `<div class="review-empty">${messages.infoNoChoicesYet || 'Choisissez jusqu\'à trois filières afin de poursuivre.'}</div>`}
                </div>
            </div>
            <div class="review-card card">
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-start gap-3">
                        <div>
                            <span class="step-pill"><i class="bi bi-file-earmark-arrow-up"></i> Documents déposés</span>
                            <h3 class="h5 mt-2 mb-1">Pièces jointes</h3>
                        </div>
                        <button type="button" class="btn btn-outline-primary btn-sm" data-step-target="4">Modifier</button>
                    </div>
                    ${documents.length ? `
                        <ul class="review-list">
                            ${documents.map(documentItem => `<li>${escapeHtml(documentItem.fileName || 'Document')}</li>`).join('')}
                        </ul>
                    ` : `<div class="review-empty">${messages.infoNoDocumentsYet || 'Aucun document n\'a encore été ajouté.'}</div>`}
                </div>
            </div>
        `;
    }

    function formatDecimal(value) {
        return value === null || value === undefined || value === '' ? '—' : String(value);
    }

    async function postJson(form, extraData = null) {
        const formData = new FormData(form);
        if (extraData) {
            Object.entries(extraData).forEach(([key, value]) => {
                if (Array.isArray(value)) {
                    value.forEach(item => formData.append(key, item));
                } else if (value !== null && value !== undefined) {
                    formData.append(key, value);
                }
            });
        }

        const response = await fetch(form.action, {
            method: 'POST',
            body: formData,
            credentials: 'same-origin',
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        });

        const payload = await response.json().catch(() => ({}));
        if (!response.ok || payload.success === false) {
            throw payload;
        }

        return payload;
    }

    function hydrateParcoursForm() {
        if (!parcoursForm) {
            return;
        }

        const parcours = wizardState.parcours || {};
        const annee = parcoursForm.querySelector('[name="AnneeBac"]');
        const serie = parcoursForm.querySelector('[name="SerieBacId"]');
        const mention = parcoursForm.querySelector('[name="MentionId"]');
        const noteNationale = parcoursForm.querySelector('[name="NoteNationale"]');
        const noteRegionale = parcoursForm.querySelector('[name="NoteRegionale"]');

        if (annee) annee.value = parcours.anneeBac || '';
        if (serie) serie.value = parcours.serieBacId || '';
        if (mention) mention.value = parcours.mentionId || '';
        if (noteNationale) noteNationale.value = parcours.noteNationale ?? '';
        if (noteRegionale) noteRegionale.value = parcours.noteRegionale ?? '';
    }

    function setProgress(text) {
        clearProgressAlerts();
        const alert = createAlert('info', text, 0);
        if (alert) {
            alert.dataset.progress = 'true';
        }
    }

    function syncChoicesFromCurrentList() {
        if (!choicesList) {
            return;
        }

        wizardState.choixFilieres = Array.from(choicesList.querySelectorAll('li.choice-item')).map((item, index) => ({
            ordreChoix: index + 1,
            filiereId: Number(item.dataset.id || 0),
            filiereNom: item.querySelector('.choice-name')?.textContent?.trim() || ''
        }));
        updateChoicesCounter();
    }

    function setupFiliereInteractions() {
        filiereCards.forEach(card => {
            const button = card.querySelector('.select-btn');
            if (!button) {
                return;
            }

            button.addEventListener('click', event => {
                event.preventDefault();
                const id = Number(card.dataset.id || 0);
                if (wizardState.choixFilieres.some(choice => choice.filiereId === id)) {
                    removeChoice(id);
                } else {
                    addChoice(card);
                }
            });
        });

        if (choicesList) {
            choicesList.addEventListener('click', event => {
                const target = event.target;
                if (!(target instanceof HTMLElement) || !target.classList.contains('remove-choice')) {
                    return;
                }

                const item = target.closest('li.choice-item');
                if (item) {
                    removeChoice(Number(item.dataset.id || 0));
                }
            });

            let draggedItem = null;
            choicesList.addEventListener('dragstart', event => {
                draggedItem = event.target instanceof HTMLElement ? event.target.closest('li.choice-item') : null;
                if (event.dataTransfer) {
                    event.dataTransfer.effectAllowed = 'move';
                }
            });

            choicesList.addEventListener('dragover', event => event.preventDefault());
            choicesList.addEventListener('drop', event => {
                event.preventDefault();
                const targetItem = event.target instanceof HTMLElement ? event.target.closest('li.choice-item') : null;
                if (!draggedItem || !targetItem || draggedItem === targetItem) {
                    return;
                }

                choicesList.insertBefore(draggedItem, targetItem.nextSibling);
                syncChoicesFromCurrentList();
                renderChoicesList();
            });
        }

        if (clearChoicesBtn) {
            clearChoicesBtn.addEventListener('click', () => {
                wizardState.choixFilieres = [];
                document.querySelectorAll('.filiere-card.selected').forEach(card => card.classList.remove('selected'));
                renderChoicesList();
                createAlert('info', 'Votre sélection a été effacée.', 5000);
            });
        }
    }


    function setupDocuments() {
        if (fileDropZone && fileInput) {
            fileDropZone.addEventListener('dragover', event => {
                event.preventDefault();
                fileDropZone.classList.add('dragover');
            });

            fileDropZone.addEventListener('dragleave', () => fileDropZone.classList.remove('dragover'));
            fileDropZone.addEventListener('drop', event => {
                event.preventDefault();
                fileDropZone.classList.remove('dragover');
                if (event.dataTransfer?.files?.length) {
                    addFilesToInput(event.dataTransfer.files);
                }
            });

            fileInput.addEventListener('change', event => {
                // On récupère les fichiers fraîchement choisis, puis on les fusionne
                // avec ceux déjà présents, au lieu de laisser le navigateur écraser.
                const newFiles = event.target.files;
                if (newFiles && newFiles.length) {
                    addFilesToInput(newFiles, /* alreadyOnInput */ true);
                } else {
                    renderFileList();
                }
            });
        }

        if (fileList) {
            fileList.addEventListener('click', event => {
                const target = event.target;
                if (!(target instanceof HTMLElement) || !target.classList.contains('remove-file')) {
                    return;
                }

                const index = Number(target.dataset.index);
                const transfer = new DataTransfer();
                Array.from(fileInput?.files || []).forEach((file, fileIndex) => {
                    if (fileIndex !== index) {
                        transfer.items.add(file);
                    }
                });

                if (fileInput) {
                    fileInput.files = transfer.files;
                }

                renderFileList();
            });
        }
    }

    // Fusionne de nouveaux fichiers avec ceux déjà sélectionnés dans l'input,
    // en évitant les doublons (même nom + même taille).
    function addFilesToInput(newFileList, alreadyOnInput = false) {
        const transfer = new DataTransfer();
        const existing = alreadyOnInput ? [] : Array.from(fileInput?.files || []);
        const seen = new Set();

        const pushUnique = file => {
            const key = `${file.name}::${file.size}::${file.lastModified}`;
            if (seen.has(key)) return;
            seen.add(key);
            transfer.items.add(file);
        };

        existing.forEach(pushUnique);
        Array.from(newFileList).forEach(pushUnique);

        if (fileInput) {
            fileInput.files = transfer.files;
        }

        renderFileList();
    }

    function setupServerAlerts() {
        const firstAlert = alertsHost?.querySelector('.alert');
        if (firstAlert) {
            requestAnimationFrame(() => firstAlert.scrollIntoView({ behavior: 'smooth', block: 'start' }));
        }

        Array.from(alertsHost?.querySelectorAll('.alert-success') || []).forEach(alert => {
            window.setTimeout(() => dismissAlert(alert), 5000);
        });

        const focusField = firstAlert?.dataset.validationFocus || '';
        if (focusField) {
            const target = document.getElementById(focusField) || document.querySelector(`[name="${focusField}"]`);
            if (target) {
                window.setTimeout(() => target.focus(), 50);
            }
        }
    }

    async function saveParcours() {
        if (!parcoursForm) {
            return false;
        }

        if (!parcoursForm.checkValidity()) {
            parcoursForm.reportValidity();
            focusFirstInvalidField(parcoursForm);
            createAlert('warning', 'Veuillez compléter les informations de votre parcours avant de continuer.', 0);
            return false;
        }

        setProgress(messages.progressSaving || 'Enregistrement en cours...');

        try {
            const payload = await postJson(parcoursForm);
            wizardState.parcours = serializeParcours();
            wizardState.currentStep = payload.nextStep || 3;
            clearProgressAlerts();
            createAlert('success', payload.message || messages.successParcoursSaved || 'Vos informations de parcours ont bien été enregistrées.', 5000);
            showStep(payload.nextStep || 3);
            return true;
        } catch (payload) {
            clearProgressAlerts();
            (payload.messages || [messages.errorUnexpected]).forEach(message => createAlert('danger', message, 0));
            return false;
        }
    }

    function serializeParcours() {
        const formData = new FormData(parcoursForm);
        return {
            anneeBac: Number(formData.get('AnneeBac') || 0),
            serieBacId: Number(formData.get('SerieBacId') || 0),
            mentionId: Number(formData.get('MentionId') || 0),
            noteNationale: toNullableNumber(formData.get('NoteNationale')),
            noteRegionale: toNullableNumber(formData.get('NoteRegionale'))
        };
    }

    function toNullableNumber(value) {
        if (value === null || value === undefined || value === '') {
            return null;
        }

        const parsed = Number(value);
        return Number.isFinite(parsed) ? parsed : null;
    }

    async function saveChoices() {
        if (!choicesForm) {
            return false;
        }

        syncChoicesFromCurrentList();
        if (wizardState.choixFilieres.length < 1 || wizardState.choixFilieres.length > 3) {
            createAlert('warning', 'Veuillez sélectionner entre une et trois filières.', 0);
            return false;
        }

        setProgress(messages.progressSaving || 'Enregistrement en cours...');

        try {
            const formData = new FormData(choicesForm);
            wizardState.choixFilieres.forEach((choice, index) => {
                formData.append(`ChoixFilieres[${index}].OrdreChoix`, String(choice.ordreChoix));
                formData.append(`ChoixFilieres[${index}].FiliereId`, String(choice.filiereId));
            });

            const response = await fetch(choicesForm.action, {
                method: 'POST',
                body: formData,
                credentials: 'same-origin',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            const payload = await response.json().catch(() => ({}));
            if (!response.ok || payload.success === false) {
                clearProgressAlerts();
                (payload.messages || [payload.message || messages.errorUnexpected]).forEach(message => createAlert('danger', message, 0));
                return false;
            }

            wizardState.currentStep = payload.nextStep || 4;
            clearProgressAlerts();
            createAlert('success', payload.message || messages.successChoicesSaved || 'Vos choix de filières ont bien été enregistrés.', 5000);
            showStep(payload.nextStep || 4);
            return true;
        } catch {
            clearProgressAlerts();
            createAlert('danger', messages.errorUnexpected || 'Une erreur inattendue est survenue. Veuillez réessayer dans quelques instants.', 0);
            return false;
        }
    }

    async function saveDocuments() {
        if (!docsForm) {
            return false;
        }

        const selectedFiles = Array.from(fileInput?.files || []);
        if (selectedFiles.length === 0 && wizardState.documents.length === 0) {
            createAlert('warning', messages.warningNoFilesSelected || 'Aucun document n\'a été sélectionné.', 0);
            return false;
        }

        if (selectedFiles.length === 0 && wizardState.documents.length > 0) {
            showStep(5);
            renderReview();
            return true;
        }

        setProgress(messages.progressUploadingDocuments || 'Téléversement des documents...');

        try {
            const formData = new FormData(docsForm);
            const response = await fetch(docsForm.action, {
                method: 'POST',
                body: formData,
                credentials: 'same-origin',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            const payload = await response.json().catch(() => ({}));
            if (!response.ok || payload.success === false) {
                clearProgressAlerts();
                (payload.messages || [payload.message || messages.errorUnexpected]).forEach(message => createAlert('danger', message, 0));
                return false;
            }

            wizardState.documents = Array.isArray(payload.data?.documents) ? payload.data.documents : (Array.isArray(payload.documents) ? payload.documents : selectedFiles.map(file => ({
                fieldName: file.name,
                fileName: file.name,
                tempFilePath: '',
                size: file.size
            })));
            wizardState.currentStep = payload.nextStep || 5;
            clearProgressAlerts();
            createAlert('success', payload.message || messages.successDocumentsUploaded || 'Vos documents ont bien été ajoutés à votre dossier.', 5000);
            if (fileInput) {
                fileInput.value = '';
            }
            renderFileList();
            showStep(payload.nextStep || 5);
            renderReview();
            return true;
        } catch {
            clearProgressAlerts();
            createAlert('danger', messages.errorUnexpected || 'Une erreur inattendue est survenue. Veuillez réessayer dans quelques instants.', 0);
            return false;
        }
    }

    function submitWithProgress() {
        if (!submitForm) {
            return;
        }

        setProgress(messages.progressSaving || 'Enregistrement en cours...');
        window.setTimeout(() => submitForm.submit(), 50);
    }

    function setupNavigation() {
        if (nextBtn) {
            nextBtn.addEventListener('click', async () => {
                if (currentStep === 2) {
                    await saveParcours();
                    return;
                }

                if (currentStep === 3) {
                    await saveChoices();
                    return;
                }

                if (currentStep === 4) {
                    await saveDocuments();
                    return;
                }

                if (currentStep === 5) {
                    showStep(6);
                    return;
                }

                if (currentStep === 6) {
                    if (!finalConfirmation || !finalConfirmation.checked) {
                        createAlert('warning', 'Veuillez confirmer la validation avant de poursuivre.', 0);
                        return;
                    }

                    if (submitConfirmModalEl) {
                        bootstrap.Modal.getOrCreateInstance(submitConfirmModalEl).show();
                        return;
                    }

                    submitWithProgress();
                    return;
                }

                showStep(currentStep + 1);
            });
        }

        if (prevBtn) {
            prevBtn.addEventListener('click', () => {
                if (currentStep > 1) {
                    showStep(currentStep - 1);
                }
            });
        }

        if (confirmSubmitBtn) {
            confirmSubmitBtn.addEventListener('click', () => submitWithProgress());
        }

        document.addEventListener('click', event => {
            const target = event.target;
            if (!(target instanceof HTMLElement)) {
                return;
            }

            if (target.matches('[data-step-target]')) {
                const step = Number(target.getAttribute('data-step-target'));
                if (step) {
                    showStep(step);
                }
            }
        });
    }

    function initializeWizard() {
        hydrateParcoursForm();
        hydrateChoicesFromState();
        renderFileList();
        setupServerAlerts();
        setupFiliereInteractions();
        setupDocuments();
        setupNavigation();
        showStep(currentStep);
    }

    initializeWizard();
});
