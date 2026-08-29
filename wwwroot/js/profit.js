// profit.js — ProfitModule  (requires Chart.js loaded before this file)

const ProfitModule = (() => {

    const _charts = {};

    const C = {
        revenue : 'rgba(49,130,206,0.85)', revenueB: 'rgba(49,130,206,1)',
        cost    : 'rgba(229,62,62,0.75)',  costB   : 'rgba(229,62,62,1)',
        profit  : 'rgba(56,161,105,0.85)', profitB : 'rgba(56,161,105,1)',
        margin  : 'rgba(128,90,213,0.9)'
    };

    // ── Monthly bar + margin line ─────────────────────────────────────────────
    function renderMonthlyChart(canvasId, data) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return;
        if (_charts[canvasId]) _charts[canvasId].destroy();

        _charts[canvasId] = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: data.labels,
                datasets: [
                    { label:'Revenue', data:data.revenue, backgroundColor:C.revenue, borderColor:C.revenueB, borderWidth:1, order:2 },
                    { label:'Cost',    data:data.cost,    backgroundColor:C.cost,    borderColor:C.costB,    borderWidth:1, order:2 },
                    { label:'Profit',  data:data.profit,  backgroundColor:C.profit,  borderColor:C.profitB,  borderWidth:1, order:2 },
                    { label:'Margin %', data:data.margin, type:'line', yAxisID:'y2',
                      borderColor:C.margin, backgroundColor:'transparent',
                      borderWidth:2, pointRadius:4, tension:0.35, order:1 }
                ]
            },
            options: {
                responsive:true, maintainAspectRatio:false,
                interaction:{ mode:'index', intersect:false },
                plugins:{
                    legend:{ position:'top' },
                    tooltip:{ callbacks:{ label(c){
                        const v = c.parsed.y ?? 0;
                        return c.dataset.label === 'Margin %'
                            ? ` ${c.dataset.label}: ${v.toFixed(1)}%`
                            : ` ${c.dataset.label}: PKR ${v.toLocaleString('en-PK',{minimumFractionDigits:2})}`;
                    }}}
                },
                scales:{
                    x:{ grid:{ display:false } },
                    y:{ beginAtZero:true, ticks:{ callback:v=>'PKR '+Number(v).toLocaleString('en-PK') } },
                    y2:{ position:'right', beginAtZero:true, grid:{ drawOnChartArea:false }, ticks:{ callback:v=>v+'%' } }
                }
            }
        });
    }

    // ── Doughnut for top products ─────────────────────────────────────────────
    function renderProductDoughnut(canvasId, labels, values) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return;
        if (_charts[canvasId]) _charts[canvasId].destroy();

        const palette = ['#3182ce','#38a169','#dd6b20','#805ad5','#e53e3e','#319795','#d69e2e','#667eea','#f6ad55','#68d391'];
        _charts[canvasId] = new Chart(ctx, {
            type:'doughnut',
            data:{ labels, datasets:[{ data:values, backgroundColor:palette.slice(0,labels.length), borderWidth:2, borderColor:'#fff' }] },
            options:{ responsive:true, maintainAspectRatio:false,
                plugins:{ legend:{ position:'right', labels:{boxWidth:12} },
                    tooltip:{ callbacks:{ label(c){ return ` ${c.label}: PKR ${(c.parsed??0).toLocaleString('en-PK',{minimumFractionDigits:2})}`; }}} }}
        });
    }

    // ── Fetch chart data then render ──────────────────────────────────────────
    async function loadAndRenderMonthlyChart(canvasId, year) {
        try {
            const res  = await fetch(`/Profit/GetChartData?year=${year}`);
            const data = await res.json();
            renderMonthlyChart(canvasId, data);
            return data;
        } catch(e) { console.error('ProfitModule chart load failed', e); return null; }
    }

    // ── Period filter (year + month selects → navigate) ───────────────────────
    function initPeriodFilter({ yearSelectId='yearSelect', monthSelectId='monthSelect', baseUrl=window.location.pathname } = {}) {
        const y = document.getElementById(yearSelectId);
        const m = document.getElementById(monthSelectId);
        const go = () => {
            const p = new URLSearchParams();
            if (y?.value) p.set('year',  y.value);
            if (m?.value) p.set('month', m.value);
            window.location.href = `${baseUrl}?${p}`;
        };
        y?.addEventListener('change', go);
        m?.addEventListener('change', go);
    }

    // ── CSV download via controller ───────────────────────────────────────────
    function exportCsv(viewName, year, month) {
        let url = `/Profit/ExportCsv?view=${viewName}&year=${year}`;
        if (month && month !== 'null') url += `&month=${month}`;
        window.location.href = url;
    }

    // ── Margin colour helper ──────────────────────────────────────────────────
    function marginClass(pct) {
        if (pct >= 30) return 'profit-margin-high';
        if (pct >= 10) return 'profit-margin-mid';
        return 'profit-margin-low';
    }

    return { loadAndRenderMonthlyChart, renderProductDoughnut, initPeriodFilter, exportCsv, marginClass };
})();
