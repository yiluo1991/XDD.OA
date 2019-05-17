/**
 * Grid-light theme for Highcharts JS
 * @author Torstein Honsi
 * @override Syne 2015-8-18
 */

// Load the fonts
Highcharts.createElement('link', {
    href: 'https://fonts.googleapis.com/css?family=Dosis:400,600',
    rel: 'stylesheet',
    type: 'text/css'
}, null, document.getElementsByTagName('head')[0]);

Highcharts.theme = {
    //colors: ["#7cb5ec", "#f7a35c", "#90ee7e", "#7798BF", "#aaeeee", "#ff0066", "#eeaaee",
    //	"#55BF3B", "#DF5353", "#7798BF", "#aaeeee"],
    chart: {
        backgroundColor: null,
        style: {
            fontFamily: "Dosis, sans-serif"
        }
    },
    title: {
        style: {
            fontSize: '16px',
            fontWeight: 'bold',
            textTransform: 'uppercase'
        }
    },
    legend: {
        itemStyle: {
            fontWeight: 'bold',
            fontSize: '13px'
        }
    },
    xAxis: {
        gridLineWidth: 1,
        labels: {
            style: {
                fontSize: '12px'
            }
        }
    },
    yAxis: {
        minorTickInterval: 'auto',
        minorGridLineDashStyle: "LongDash",
        title: {
            style: {
                textTransform: 'uppercase'
            }
        },
        labels: {
            style: {
                fontSize: '12px'
            }
        }
    },
    plotOptions: {
        candlestick: {
            lineColor: '#404048'
        }
    },
    rangeSelector: {
        inputDateFormat: '%Y-%m-%d',
        buttons: [{//定义一组buttons,下标从0开始
            type: 'week',
            count: 1,
            text: '一周'
        }, {
            type: 'month',
            count: 1,
            text: '一月'
        }, {
            type: 'month',
            count: 3,
            text: '三月'
        }, {
            type: 'month',
            count: 6,
            text: '半年'
        }, {
            type: 'ytd',
            text: '今年'
        }, {
            type: 'year',
            count: 1,
            text: '一年'
        }, {
            type: 'all',
            text: '全部'
        }],
        selected: 1
    },
    scrollbar: {
        height: 9
    },
    navigator: {
        height: 30,
        xAxis: {
            labels: {
                formatter: function () {
                    var vDate = new Date(this.value);
                    return vDate.getFullYear() + "-" + (vDate.getMonth() + 1) + "-" + vDate.getDate();
                },
                align: 'center'
            }
        }, yAxis: {
            minorTickInterval: null
        }, margin: 10,
        series: {
            fillOpacity: 0.05,
            dataGrouping: {
                smoothed: true
            },
            lineWidth: 1,
            marker: {
                enabled: false
            }
        }
    },
    // General
    background2: '#F0F0EA',
    lang: {
        rangeSelectorFrom: '从',
        rangeSelectorTo: '到',
        rangeSelectorZoom: '范围',
        months: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月'],
        shortMonths: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月'],
        weekdays: ['星期天', '星期一', '星期二', '星期三', '星期四', '星期五', '星期六']
    },
    tooltip: {

        borderWidth: 0,
        backgroundColor: 'rgba(255,255,255,0.8)',
        shadow: true,
        headerFormat:'<span style="font-size: 12px">{point.key}</span><br/>',
        dateTimeLabelFormats: { millisecond: "%A, %b %e, %H:%M:%S.%L", second: "%A, %b %e, %H:%M:%S", minute: "%A, %b %e, %H:%M", hour: "%A, %b %e, %H:%M", day: '%Y-%m-%d, %A', week: "Week from %A, %b %e, %Y", month: "%B %Y", year: "%Y" }
    }
};

// Apply the theme
Highcharts.setOptions(Highcharts.theme);
