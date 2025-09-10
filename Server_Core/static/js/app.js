// File: static/js/app.js
/**
 * BMS Bridge Application - Final Refactored Version
 * Includes Kneeboards V2 with PDF support and config loading.
 */
class BMSBridgeApp {
    constructor() {
        this.elements = this._initializeDOMElements();
        this.state = this._initializeState();
        this.config = {
            retryDelay: 1000,
            websocketReconnectDelay: 5000,
            healthCheckInterval: 15000,
            pdfRenderScale: 2.0 // Higher scale for better quality on high-res screens
        };
        this.init();
    }

    _initializeDOMElements() {
        const requiredElements = {
            contentContainer: '#content-container', loadingOverlay: '#loading-overlay',
            loadingMessage: '#loading-message', errorNotification: '#error-notification',
            errorMessage: '#error-message', connectionIndicator: '#connection-indicator',
            connectionText: '#connection-text'
        };
        const elements = {};
        for (const [key, selector] of Object.entries(requiredElements)) {
            elements[key] = document.querySelector(selector);
            if (!elements[key]) throw new Error(`Fatal Error: DOM element '${selector}' not found.`);
        }
        return elements;
    }

    _initializeState() {
        return {
            currentTab: null,
            lastViewedPages: this._loadStateFromStorage(),
            websocket: null,
            currentKeyHandler: null,
            isConnected: false,
            healthCheckTimer: null,
            isLoading: false,
            websocketReconnectTimer: null,
            pdfLibrary: null // To store the loaded pdf.js library
        };
    }

    async _loadPdfLibrary() {
        if (this.state.pdfLibrary) return;
        try {
            this.state.pdfLibrary = await import('/libs/pdfjs/build/pdf.mjs');
            this.state.pdfLibrary.GlobalWorkerOptions.workerSrc = '/libs/pdfjs/build/pdf.worker.mjs';
            console.log("pdf.js library loaded successfully.");
        } catch (e) {
            console.error("Fatal: Could not load pdf.js library.", e);
            this.showError("Failed to load core PDF components. Please refresh.", 0);
        }
    }

    init() {
        this._loadPdfLibrary();
        this._startHealthCheck();
        this._setupTabNavigation();
        const firstTab = document.querySelector('.tab-button');
        if (firstTab) firstTab.click();
        window.addEventListener('beforeunload', () => this.cleanupCurrentView());
    }

    _setupTabNavigation() {
        document.querySelectorAll('.tab-button').forEach(button => {
            button.addEventListener('click', () => {
                const tab = button.dataset.tab;
                switch (tab) {
                    case 'left-kneeboard':
                        this.loadKneeboard('Left', button);
                        break;
                    case 'right-kneeboard':
                        this.loadKneeboard('Right', button);
                        break;
                    case 'briefing':
                        this.loadHtmlBriefing(button);
                        break;
                    case 'charts':
                        this.loadUnderConstruction('Charts', button);
                        break;
                    case 'procedure':
                        const procedureData = [{ path: '/procedure/sample_procedure.pdf', type: 'pdf' }];
                        this._setupConfigurableViewer('Docs', procedureData);
                        break;
                }
            });
        });
    }

    _startHealthCheck() {
        this.checkHealth();
        this.state.healthCheckTimer = setInterval(() => this.checkHealth(), this.config.healthCheckInterval);
    }

    async checkHealth() {
        try {
            const response = await fetch('/api/health', { cache: 'no-cache' });
            if (!response.ok) throw new Error(`Status: ${response.status}`);
            const health = await response.json();
            this.updateConnectionStatus(health.bms_status === "CONNECTED");
        } catch (error) {
            console.warn('Health check failed:', error);
            this.updateConnectionStatus(false);
        }
    }

    updateConnectionStatus(connected) {
        if (this.state.isConnected === connected) return;
        this.state.isConnected = connected;
        this.elements.connectionIndicator.className = connected ? 'status-indicator connected' : 'status-indicator disconnected';
        this.elements.connectionText.textContent = connected ? 'BMS Connected' : 'BMS Disconnected';
    }

    _loadStateFromStorage() {
        try {
            const savedState = localStorage.getItem('bmsBridgeState');
            const defaults = { 'left-kneeboard': 0, 'right-kneeboard': 0, 'procedure': 1 };
            return savedState ? { ...defaults, ...JSON.parse(savedState) } : defaults;
        } catch (e) {
            console.warn("Could not load state from localStorage.", e);
            return { 'left-kneeboard': 0, 'right-kneeboard': 0, 'procedure': 1 };
        }
    }

    _saveStateToStorage() {
        try {
            localStorage.setItem('bmsBridgeState', JSON.stringify(this.state.lastViewedPages));
        } catch (e) {
            console.warn("Could not save state to localStorage.", e);
        }
    }

    showLoading(message = 'Loading...') {
        if (this.state.isLoading) {
            this.elements.loadingMessage.textContent = message;
            return;
        }
        this.state.isLoading = true;
        this.elements.loadingMessage.textContent = message;
        this.elements.loadingOverlay.classList.remove('hidden');
    }

    hideLoading() {
        this.state.isLoading = false;
        this.elements.loadingOverlay.classList.add('hidden');
    }

    showError(message, duration = 5000) {
        this.elements.errorMessage.textContent = message;
        this.elements.errorNotification.classList.remove('hidden');
        if (duration > 0) {
            setTimeout(() => this.hideError(), duration);
        }
    }

    hideError() {
        this.elements.errorNotification.classList.add('hidden');
    }

    setActiveTab(element) {
        document.querySelectorAll('.tab-button').forEach(btn => btn.classList.remove('active'));
        element.classList.add('active');
        this.state.currentTab = element.dataset.tab;
    }

    cleanupCurrentView() {
        if (this.state.currentKeyHandler) {
            document.removeEventListener('keydown', this.state.currentKeyHandler);
            this.state.currentKeyHandler = null;
        }
    }
    
    // --- NEW: Universal Kneeboard Loader ---
    async loadKneeboard(boardName, element) {
        this.cleanupCurrentView();
        this.setActiveTab(element);
        this.showLoading(`Loading ${boardName} kneeboard...`);
        try {
            const response = await fetch(`/api/kneeboards/${boardName.toLowerCase()}`);
            if (!response.ok) throw new Error(`Server returned status ${response.status}`);
            const result = await response.json();

            if (!result.success || result.items.length === 0) {
                this.elements.contentContainer.innerHTML = `<div class="empty-content"><h2>No ${boardName} Kneeboard Pages</h2><p>Add and enable files in the launcher.</p></div>`;
                return;
            }
            await this._setupConfigurableViewer(boardName, result.items);
        } catch (error) {
            this.showError(`Failed to load kneeboard: ${error.message}`);
            this.elements.contentContainer.innerHTML = `<div class="empty-content"><h2>Error loading kneeboard</h2><p>${error.message}</p></div>`;
        } finally {
            this.hideLoading();
        }
    }

    async loadHtmlBriefing(element) {
        this.cleanupCurrentView();
        this.setActiveTab(element);
        this.showLoading('Loading Briefing...');
        
        try {
            const response = await fetch('/api/briefing/html');
            
            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(`Server error: ${response.status} - ${errorText}`);
            }
            
            const briefingBodyHtml = await response.text();
            
            this.elements.contentContainer.innerHTML = `
                <div class="briefing-container">
                    <div class="briefing-content">
                        ${briefingBodyHtml}
                    </div>
                </div>
            `;
            
        } catch (error) {
            console.error('Failed to load HTML briefing:', error);
            this.showError(error.message);
            this.elements.contentContainer.innerHTML = `<div class="empty-content"><h2>Error loading briefing</h2><p>${error.message}</p></div>`;
        } finally {
            this.hideLoading();
        }
    }

    // --- NEW: Powerful viewer for images and PDFs ---
    async _setupConfigurableViewer(name, items) {
        this.showLoading('Preparing kneeboard pages...');

        this.elements.contentContainer.innerHTML = `
            <div class="image-viewer">
                <div class="viewer-controls">
                    <button id="prev-btn" class="control-btn">&lt; Prev</button>
                    <span id="page-display" class="page-info"></span>
                    <button id="next-btn" class="control-btn">Next &gt;</button>
                </div>
                <div class="slider-container">
                    <input type="range" min="1" max="100" value="1" id="page-slider">
                </div>
                <div class="viewer-content" id="viewer-content-area">
                    <!-- This area will be dynamically populated -->
                </div>
            </div>`;

        const viewerContent = document.getElementById('viewer-content-area');
        const pageDisplay = document.getElementById('page-display');
        const pageSlider = document.getElementById('page-slider');
        const pdfjsLib = this.state.pdfLibrary;

        if (!pdfjsLib) {
            this.showError("PDF library is not ready. Please wait or refresh.", 5000);
            return;
        }

        const pageList = [];
        const pdfDocsCache = {};

        for (const [index, item] of items.entries()) {
            this.showLoading(`Processing file ${index + 1} / ${items.length}...`);
            if (item.type === 'image') {
                pageList.push({ type: 'image', path: item.path });
            } else if (item.type === 'pdf') {
                try {
                    if (!pdfDocsCache[item.path]) {
                        pdfDocsCache[item.path] = await pdfjsLib.getDocument(item.path).promise;
                    }
                    const pdfDoc = pdfDocsCache[item.path];
                    for (let i = 1; i <= pdfDoc.numPages; i++) {
                        pageList.push({ type: 'pdf', pageNum: i, doc: pdfDoc });
                    }
                } catch (e) {
                    console.error(`Failed to load PDF ${item.path}`, e);
                }
            }
        }

        if (pageList.length === 0) {
            this.elements.contentContainer.innerHTML = `<div class="empty-content"><h2>No valid pages found.</h2><p>Check if the configured files exist.</p></div>`;
            this.hideLoading();
            return;
        }

        pageSlider.max = pageList.length;
        if (pageList.length <= 1) {
            document.querySelector('.slider-container').style.display = 'none';
        }

        let currentPageIndex = this.state.lastViewedPages[name.toLowerCase()] || 0;
        let debounceTimer = null;
        if (currentPageIndex >= pageList.length) {
            currentPageIndex = 0;
        }

        const renderPage = async (index) => {
            const page = pageList[index];
            this.showLoading('Rendering page...');
            viewerContent.innerHTML = ''; 

            if (page.type === 'image') {
                const img = document.createElement('img');
                img.id = 'current-image';
                img.src = page.path;
                viewerContent.appendChild(img);
            } else if (page.type === 'pdf') {
                const canvas = document.createElement('canvas');
                canvas.id = 'pdf-canvas';
                viewerContent.appendChild(canvas);
                const pdfPage = await page.doc.getPage(page.pageNum);
                const viewport = pdfPage.getViewport({ scale: this.config.pdfRenderScale });
                canvas.height = viewport.height;
                canvas.width = viewport.width;
                await pdfPage.render({ canvasContext: canvas.getContext('2d'), viewport }).promise;
            }
            
            pageDisplay.textContent = `${index + 1} / ${pageList.length}`;
            pageSlider.value = index + 1;
            currentPageIndex = index;
            this.state.lastViewedPages[name.toLowerCase()] = currentPageIndex;
            this._saveStateToStorage();
            this.hideLoading();
        };

        const prev = () => { if (currentPageIndex > 0) renderPage(currentPageIndex - 1); };
        const next = () => { if (currentPageIndex < pageList.length - 1) renderPage(currentPageIndex + 1); };

        pageSlider.addEventListener('input', (e) => {
                const newIndex = parseInt(e.target.value) - 1;
                
                pageDisplay.textContent = `${e.target.value} / ${pageList.length}`;

                clearTimeout(debounceTimer);

                debounceTimer = setTimeout(() => {
                    renderPage(newIndex);
                }, 100); 
            });

        document.getElementById('prev-btn').onclick = prev;
        document.getElementById('next-btn').onclick = next;
        this.state.currentKeyHandler = e => { if (e.key === 'ArrowLeft') prev(); if (e.key === 'ArrowRight') next(); };
        document.addEventListener('keydown', this.state.currentKeyHandler);
        
        this._setupViewerInteractions(viewerContent, next, prev);

        renderPage(currentPageIndex);
    }

    _setupViewerInteractions(element, nextCallback, prevCallback) {
        if (!element) return;
        let touchStartX = 0;
        element.addEventListener('wheel', (e) => {
            e.preventDefault();
            if (e.deltaY > 0) nextCallback();
            else prevCallback();
        }, { passive: false });
        element.addEventListener('touchstart', (e) => {
            touchStartX = e.touches[0].clientX;
        }, { passive: true });
        element.addEventListener('touchend', (e) => {
            const swipeDistance = e.changedTouches[0].clientX - touchStartX;
            if (swipeDistance < -50) nextCallback();
            else if (swipeDistance > 50) prevCallback();
        }, { passive: true });
    }

    loadUnderConstruction(pageTitle, element) {
        this.cleanupCurrentView();
        this.setActiveTab(element);
        this.elements.contentContainer.innerHTML = `
            <div class="empty-content">
                <h1 style="font-size: 3rem; margin-bottom: 1rem;">🚧</h1>
                <h2>${pageTitle} - Under Construction</h2>
                <p style="margin-top: 0.5rem; color: var(--text-secondary);">This feature is coming soon!</p>
            </div>`;
    }
}


let app;
try {
    app = new BMSBridgeApp();
} catch (error) {
    console.error("Failed to initialize BMS Bridge App:", error);
    document.body.innerHTML = `<div style="padding: 2rem; text-align: center; font-family: sans-serif;"><h1>Application failed to start</h1><p>${error.message}</p></div>`;
}

// --- PWA Service Worker Registration ---
if ('serviceWorker' in navigator) {
    window.addEventListener('load', () => {
        navigator.serviceWorker.register('/static/js/sw.js')
            .then(registration => {
                console.log('ServiceWorker registration successful with scope: ', registration.scope);
            })
            .catch(err => {
                console.error('ServiceWorker registration failed: ', err);
            });
    });
}