// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { customElement, inject, processContent, ViewCompiler, ViewResources, BehaviorInstruction, TargetInstruction, ViewFactory } from 'aurelia-framework';

import { Chart, ChartConfiguration } from 'chart.js';

@inject(TargetInstruction)
@customElement('line-chart')
@processContent(LineChart.processContent)
export class LineChart {
    attached() {
        // const viewFactory = (targetInstruction.elementInstruction.type as any).viewFactory as ViewFactory;
        const canvas = document.querySelector('canvas');
        // (viewFactory as any).template.querySelector('canvas');

        if (canvas) {

            const fontFamily = window.getComputedStyle(canvas).fontFamily;
            console.log(fontFamily);

            Chart.defaults.global.defaultFontColor = '#fff';
            Chart.defaults.global.defaultFontFamily = fontFamily;
            Chart.defaults.global.defaultFontSize = 12;
            Chart.defaults.global.defaultFontStyle = 'bold';


            const context = canvas?.getContext('2d');
            if (context) {
                const chart = new Chart(context, {
                    type: 'line',
                    data: {
                        labels: ['2020/2/25', '2020/2/26', '2020/2/27', '2020/2/28', '2020/2/29', '2020/3/1', '2020/3/2'],
                        datasets: [{
                            label: 'CustomerCreated',
                            backgroundColor: 'rgb(51, 185, 230)',
                            borderColor: 'rgb(0, 67, 102)',
                            data: [4, 10, 5, 2, 20, 30, 45],
                            fill: false
                        }, {
                            label: 'OfferAccepted',
                            backgroundColor: 'rgb(51, 185, 230)',
                            borderColor: 'rgb(255, 207, 0)',
                            data: [0, 0, 3, 8, 2, 3, 5],
                            fill: false
                        }]
                    },

                    // Configuration options go here
                    options: {
                        hover: {
                            intersect: true,
                            mode: 'nearest'
                        },
                        title: {
                            display: true,
                            text: 'Number of events'
                        },
                        tooltips: {
                            intersect: false,
                            mode: 'index'
                        },
                        scales: {
                            xAxes: [{
                                gridLines: {
                                    display: true,
                                    color: 'rgb(82, 90, 99)'
                                },
                                scaleLabel: {
                                    display: true,
                                    labelString: 'Day'
                                }
                            }],
                            yAxes: [{
                                gridLines: {
                                    display: true,
                                    color: 'rgb(82, 90, 99)'
                                },
                                ticks: {
                                    beginAtZero: true
                                },
                                scaleLabel: {
                                    display: true,
                                    labelString: 'Events'
                                }
                            }],
                        }
                    }
                });
            }
        }
    }

    static processContent(compiler: ViewCompiler, resources: ViewResources, node: HTMLElement, instruction: BehaviorInstruction) {
    }
}
