// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { customElement, inject, processContent, ViewCompiler, ViewResources, BehaviorInstruction, TargetInstruction, ViewFactory } from 'aurelia-framework';

import { Chart, ChartConfiguration } from 'chart.js';

@inject(TargetInstruction)
@customElement('doughnut-chart')
@processContent(LineChart.processContent)
export class LineChart {
    attached() {
        // const viewFactory = (targetInstruction.elementInstruction.type as any).viewFactory as ViewFactory;
        const canvas = document.getElementById('doughnut') as HTMLCanvasElement;
        // (viewFactory as any).template.querySelector('canvas');

        if (canvas) {

            const fontFamily = window.getComputedStyle(canvas).fontFamily;

            Chart.defaults.global.defaultFontColor = '#fff';
            Chart.defaults.global.defaultFontFamily = fontFamily;
            Chart.defaults.global.defaultFontSize = 12;
            Chart.defaults.global.defaultFontStyle = 'bold';


            const context = canvas?.getContext('2d');
            if (context) {
                const chart = new Chart(context, {
                    type: 'doughnut',
                    data: {
                        labels: ['Success', 'Failing'],
                        datasets: [{
                            label: 'Succeeding',
                            data: [83, 21],
                            backgroundColor: [
                                'rgba(255, 206, 86, 0.2)',
                                'rgba(255, 99, 132, 0.2)'
                            ],
                            borderColor: [
                                'rgba(255, 206, 86, 1)',
                                'rgba(255, 99, 132, 1)'
                            ],
                            borderWidth: 1
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
                            text: 'Partitions'
                        },
                        tooltips: {
                            intersect: false,
                            mode: 'index'
                        }
                    }
                });
            }
        }
    }

    static processContent(compiler: ViewCompiler, resources: ViewResources, node: HTMLElement, instruction: BehaviorInstruction) {
    }
}
