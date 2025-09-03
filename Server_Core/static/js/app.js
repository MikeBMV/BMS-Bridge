// File: static/js/app.js
/**
 * BMS Bridge Application - Final Refactored Version
 * Includes all professional enhancements and restored wheel/swipe navigation.
 */
class BMSBridgeApp {
    constructor() {
        this.elements = this._initializeDOMElements();
        this.state = this._initializeState();
        this.config = {
            retryDelay: 1000,
            websocketReconnectDelay: 5000,
            healthCheckInterval: 15000
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
            websocketReconnectTimer: null
        };
    }

    _loadStateFromStorage() {
        try {
            const savedState = localStorage.getItem('bmsBridgeState');
            const defaults = { Left: 1, procedure: 1, Right: 1, briefing: 1 };
            return savedState ? { ...defaults, ...JSON.parse(savedState) } : defaults;
        } catch (e) {
            console.warn("Could not load state from localStorage.", e);
            return { Left: 1, procedure: 1, Right: 1, briefing: 1 };
        }
    }

    _saveStateToStorage() {
        try {
            localStorage.setItem('bmsBridgeState', JSON.stringify(this.state.lastViewedPages));
        } catch (e) {
            console.warn("Could not save state to localStorage.", e);
        }
    }

    init() {
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
                        this.loadUnderConstruction('Briefing', button);
                        break;
                    case 'charts':
                        this.loadUnderConstruction('Charts', button);
                        break;
                    case 'procedure':
                        this.loadPdf('procedure', button);
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
            // LOGIC FIX: The API now returns a detailed status object.
            // We check if the BMS status is "CONNECTED".
            this.updateConnectionStatus(health.bms_status === "CONNECTED");
        } catch (error) {
            console.warn('Health check failed:', error);
            this.updateConnectionStatus(false);
        }
    }

    updateConnectionStatus(connected) {
        if (this.state.isConnected === connected) return;
        this.state.isConnected = connected;
        if (connected) {
            this.elements.connectionIndicator.className = 'status-indicator connected';
            this.elements.connectionText.textContent = 'BMS Connected';
        } else {
            this.elements.connectionIndicator.className = 'status-indicator disconnected';
            this.elements.connectionText.textContent = 'BMS Disconnected';
        }
    }

    showLoading(message = 'Loading...') {
        if (this.state.isLoading) return;
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
        this._closeWebSocket();
        if (this.state.currentKeyHandler) {
            document.removeEventListener('keydown', this.state.currentKeyHandler);
            this.state.currentKeyHandler = null;
        }
    }

    async loadKneeboard(boardName, element) {
        this.cleanupCurrentView();
        this.setActiveTab(element);
        this.showLoading(`Updating ${boardName} kneeboard...`);
        try {
            await fetch('/api/kneeboards/refresh', { method: 'POST' });
            this.showLoading(`Loading ${boardName} images...`);
            const response = await fetch(`/api/kneeboard_images/${boardName}`);
            const result = await response.json();
            if (!result.success || result.images.length === 0) {
                this.elements.contentContainer.innerHTML = `<div class="empty-content"><h2>No ${boardName} Images Found</h2></div>`;
                return;
            }
            this._setupImageViewer(boardName, result.images);
        } catch (error) {
            this.showError(`Failed to load kneeboard: ${error.message}`);
        } finally {
            this.hideLoading();
        }
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

    async loadPdf(pageName, element) {
        this.cleanupCurrentView();
        this.setActiveTab(element);
        this.showLoading('Loading PDF...');
        try {
            const pdfjsLib = await import('/libs/pdfjs/build/pdf.mjs');
            pdfjsLib.GlobalWorkerOptions.workerSrc = '/libs/pdfjs/build/pdf.worker.mjs';
            const pdfDoc = await pdfjsLib.getDocument(`/${pageName}/sample_procedure.pdf`).promise;
            this._setupPdfViewer(pdfDoc, pageName);
        } catch (error) {
            this.showError(`Failed to load PDF: ${error.message}`);
        } finally {
            this.hideLoading();
        }
    }
    
    _setupImageViewer(name, images) {
        let pageNum = this.state.lastViewedPages[name] || 1;
        if (pageNum > images.length) pageNum = 1;
        this.elements.contentContainer.innerHTML = `<div class="image-viewer"><div class="viewer-controls"><button id="prev-btn" class="control-btn">&lt; Prev</button><span id="page-display" class="page-info"></span><button id="next-btn" class="control-btn">Next &gt;</button></div><div class="viewer-content"><img id="current-image" alt="Kneeboard image"/></div></div>`;
        const img = document.getElementById('current-image'), pageDisplay = document.getElementById('page-display');
        
        const update = () => {
            img.src = `/${name}/${images[pageNum - 1]}`;
            pageDisplay.textContent = `${pageNum} / ${images.length}`;
            this.state.lastViewedPages[name] = pageNum;
            this._saveStateToStorage();
        };
        const prev = () => { if (pageNum > 1) { pageNum--; update(); } };
        const next = () => { if (pageNum < images.length) { pageNum++; update(); } };

        document.getElementById('prev-btn').onclick = prev;
        document.getElementById('next-btn').onclick = next;
        this.state.currentKeyHandler = e => { if (e.key === 'ArrowLeft') prev(); if (e.key === 'ArrowRight') next(); };
        document.addEventListener('keydown', this.state.currentKeyHandler);

        const viewerContent = this.elements.contentContainer.querySelector('.viewer-content');
        this._setupViewerInteractions(viewerContent, next, prev);
        
        update();
    }

    _setupBriefingViewer(pages) {
        let pageNum = this.state.lastViewedPages.briefing || 1;
        if (pageNum > pages.length) pageNum = 1;
        this.elements.contentContainer.innerHTML = `<div class="briefing-viewer"><div class="viewer-controls"><button id="prev-btn" class="control-btn">&lt; Prev</button><span id="page-display" class="page-info"></span><button id="next-btn" class="control-btn">Next &gt;</button></div><div class="viewer-content" id="briefing-content-wrapper"></div></div>`;
        const pageDisplay = document.getElementById('page-display'), contentWrapper = document.getElementById('briefing-content-wrapper');
        
        const update = () => {
            const currentPage = pages[pageNum - 1];
            pageDisplay.textContent = `${pageNum} / ${pages.length} | ${currentPage.title}`;
            let html = '<div class="briefing-page">';
            for (const section of currentPage.sections) {
                html += `<div class="briefing-section"><h3 class="section-title">${section.name}</h3>${this._renderSection(section)}</div>`;
            }
            html += '</div>';
            contentWrapper.innerHTML = html;
            contentWrapper.scrollTop = 0;
            this.state.lastViewedPages.briefing = pageNum;
            this._saveStateToStorage();
        };
        const prev = () => { if (pageNum > 1) { pageNum--; update(); } };
        const next = () => { if (pageNum < pages.length) { pageNum++; update(); } };

        document.getElementById('prev-btn').onclick = prev;
        document.getElementById('next-btn').onclick = next;
        this.state.currentKeyHandler = e => { if (e.key === 'ArrowLeft') prev(); if (e.key === 'ArrowRight') next(); };
        document.addEventListener('keydown', this.state.currentKeyHandler);
        
        this._setupViewerInteractions(contentWrapper, next, prev);

        update();
    }

    _renderSection(section) {
        switch (section.type) {
            case 'table':
                const headers = section.data.headers;
                const tableClass = section.className ? `bms-table ${section.className}` : 'bms-table';
                return `<div class="table-container"><table class="${tableClass}"><thead><tr>${headers.map(h => `<th>${h}</th>`).join('')}</tr></thead><tbody>${section.data.rows.map(row => `<tr>${headers.map(h => `<td>${row[h] || ''}</td>`).join('')}</tr>`).join('')}</tbody></table></div>`;
            case 'kv_list': return `<ul class="kv-list">${Object.entries(section.data).map(([key, value]) => `<li><strong>${key}:</strong> <span>${value}</span></li>`).join('')}</ul>`;
            case 'text': return `<pre class="text-content">${section.data}</pre>`;
            case 'complex_section': return section.data.map(sub => `<div class="sub-section"><h4 class="sub-section-title">${sub.title}</h4>${this._renderSection(sub)}</div>`).join('');
            default: return '<p>Unknown content type</p>';
        }
    }

    _setupPdfViewer(pdfDoc, pageName) {
        this.elements.contentContainer.innerHTML = `<div class="pdf-viewer"><div class="viewer-controls"><button id="prev-btn" class="control-btn">&lt; Prev</button><span id="page-display" class="page-info"></span><button id="next-btn" class="control-btn">Next &gt;</button></div><div class="viewer-content"><canvas id="pdf-canvas"></canvas></div></div>`;
        let pageNum = this.state.lastViewedPages[pageName] || 1;
        if (pageNum > pdfDoc.numPages) pageNum = 1;
        const canvas = document.getElementById('pdf-canvas'), pageDisplay = document.getElementById('page-display');
        
        const render = async () => {
            const page = await pdfDoc.getPage(pageNum);
            const viewport = page.getViewport({ scale: 1.8 });
            canvas.height = viewport.height;
            canvas.width = viewport.width;
            await page.render({ canvasContext: canvas.getContext('2d'), viewport }).promise;
            pageDisplay.textContent = `${pageNum} / ${pdfDoc.numPages}`;
            this.state.lastViewedPages[pageName] = pageNum;
            this._saveStateToStorage();
        };
        const prev = () => { if (pageNum > 1) { pageNum--; render(); } };
        const next = () => { if (pageNum < pdfDoc.numPages) { pageNum++; render(); } };
        
        document.getElementById('prev-btn').onclick = prev;
        document.getElementById('next-btn').onclick = next;
        this.state.currentKeyHandler = e => { if (e.key === 'ArrowLeft') prev(); if (e.key === 'ArrowRight') next(); };
        document.addEventListener('keydown', this.state.currentKeyHandler);
        
        const viewerContent = this.elements.contentContainer.querySelector('.viewer-content');
        this._setupViewerInteractions(viewerContent, next, prev);
        
        render();
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
    
    _setupWebSocket() {
        if (this.state.websocket) return;
        const wsProtocol = window.location.protocol === 'https:' ? 'wss:' : 'ws:';
        const wsUrl = `${wsProtocol}//${window.location.host}/ws/flight_data`;
        this.state.websocket = new WebSocket(wsUrl);
        
        this.state.websocket.onopen = () => {
            this.hideLoading();
            console.log('WebSocket connected.');
            if (this.state.websocketReconnectTimer) {
                clearTimeout(this.state.websocketReconnectTimer);
                this.state.websocketReconnectTimer = null;
            }
        };
        
        this.state.websocket.onmessage = (event) => {
            const msg = JSON.parse(event.data);
            const grid = document.getElementById('instruments-grid');
            if (!grid) return;
            if (msg.success && msg.data) {
                grid.innerHTML = Object.entries(msg.data)
                    .filter(([key, value]) => !key.startsWith('_') && (typeof value !== 'string' || value.trim() !== ''))
                    .map(([key, value]) => `<div class="data-field"><div class="field-name">${key.replace(/([A-Z])/g, ' $1').replace(/^./, s => s.toUpperCase())}</div><div class="field-value">${typeof value === 'number' ? value.toFixed(3) : String(value)}</div></div>`)
                    .join('');
            } else {
                grid.innerHTML = `<div class="empty-content"><h2>${msg.error || "Waiting for data..."}</h2></div>`;
            }
        };
        
        this.state.websocket.onclose = () => {
            console.warn('WebSocket disconnected.');
            this.state.websocket = null;
            if (this.state.currentTab === 'instruments') this._reconnectWebSocket();
        };
        
        this.state.websocket.onerror = (err) => {
            console.error('WebSocket error:', err);
            this.showError("WebSocket connection error.", 3000);
            this.state.websocket = null;
            if (this.state.currentTab === 'instruments') this._reconnectWebSocket();
        };
    }
    
    _reconnectWebSocket() {
        if (this.state.websocketReconnectTimer) return;
        this.state.websocketReconnectTimer = setTimeout(() => {
            if (this.state.currentTab === 'instruments' && !this.state.websocket) {
                console.log("Attempting to reconnect WebSocket...");
                this._setupWebSocket();
            }
            this.state.websocketReconnectTimer = null;
        }, this.config.websocketReconnectDelay);
    }
    
    _closeWebSocket() {
        if (this.state.websocketReconnectTimer) {
            clearTimeout(this.state.websocketReconnectTimer);
            this.state.websocketReconnectTimer = null;
        }
        if (this.state.websocket) {
            this.state.websocket.close(1000, "Client initiated disconnect");
            this.state.websocket = null;
        }
    }
}

let app;
try {
    app = new BMSBridgeApp();
} catch (error) {
    console.error("Failed to initialize BMS Bridge App:", error);
    document.body.innerHTML = `<div style="padding: 2rem; text-align: center; font-family: sans-serif;"><h1>Application failed to start</h1><p>${error.message}</p></div>`;
}