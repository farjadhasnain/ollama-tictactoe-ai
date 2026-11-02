const mode = document.querySelector('#mode');
function updateMode() {
    const againstAi = mode.value === 'ai';
    document.querySelector('[data-human]').hidden = againstAi;
    document.querySelector('[data-ai]').hidden = !againstAi;
    document.querySelector('#name2').required = !againstAi;
}
mode.addEventListener('change', updateMode);
updateMode();

let submitting = false;
for (const form of document.querySelectorAll('form')) {
    form.addEventListener('submit', event => {
        if (submitting) { event.preventDefault(); return; }
        submitting = true;
        if (form.hasAttribute('data-retry') || (form.hasAttribute('data-move') && form.dataset.aiOpponent === 'true')) {
            document.querySelector('[data-status]').textContent = 'AI is thinking…';
        }
        document.body.setAttribute('aria-busy', 'true');
        requestAnimationFrame(() => {
            document.querySelectorAll('button').forEach(button => button.disabled = true);
        });
    });
}
window.addEventListener('pageshow', event => { if (event.persisted) location.reload(); });
