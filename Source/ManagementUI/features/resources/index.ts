// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { FrameworkConfiguration } from 'aurelia-framework';
import { PLATFORM } from 'aurelia-pal';
import { createTheme, loadTheme } from 'office-ui-fabric-react/lib/Styling';

import { initializeIcons } from '@uifabric/icons';

initializeIcons();

const myTheme = createTheme({
    palette: {
        themePrimary: '#ffcf00',
        themeLighterAlt: '#fffdf5',
        themeLighter: '#fff8d6',
        themeLight: '#fff1b3',
        themeTertiary: '#ffe366',
        themeSecondary: '#ffd61f',
        themeDarkAlt: '#e6bb00',
        themeDark: '#c29e00',
        themeDarker: '#8f7500',
        neutralLighterAlt: '#292d32',
        neutralLighter: '#292c31',
        neutralLight: '#272a2f',
        neutralQuaternaryAlt: '#24282c',
        neutralQuaternary: '#23262a',
        neutralTertiaryAlt: '#212428',
        neutralTertiary: '#c8c8c8',
        neutralSecondary: '#d0d0d0',
        neutralPrimaryAlt: '#dadada',
        neutralPrimary: '#ffffff',
        neutralDark: '#f4f4f4',
        black: '#f8f8f8',
        white: '#2b2f34',
    }
});

loadTheme(myTheme);

export function configure(config: FrameworkConfiguration) {
    config.globalResources([
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Utilities/DuMarqueeSelection'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/BasicInputs/DuActionButton'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/BasicInputs/DuCheckbox'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/BasicInputs/DuChoiceGroup'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/BasicInputs/DuComboBox'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/BasicInputs/DuCommandBarButton'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/BasicInputs/DuCompoundButton'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/BasicInputs/DuContextualMenu'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/BasicInputs/DuDefaultButton'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/BasicInputs/DuDropdown'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/BasicInputs/DuIconButton'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/BasicInputs/DuLabel'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/BasicInputs/DuSlider'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/BasicInputs/DuSpinButton'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/BasicInputs/DuTextField'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/BasicInputs/DuToggle'),

        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Content/DuDetailsList'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Content/DuFacepile'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Content/DuGroupedList'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Content/DuPersona'),

        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Navigation/DuBreadcrumb'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Navigation/DuCommandBar'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Navigation/DuNav'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Navigation/DuPivot'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Navigation/DuSearchBox'),

        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Pickers/DuColorPicker'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Pickers/DuCompactPeoplePicker'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Pickers/DuDatePicker'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Pickers/DuListPeoplePicker'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Pickers/DuNormalPeoplePicker'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Pickers/DuTagPicker'),

        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/ProgressValidation/DuMessageBar'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/ProgressValidation/DuProgressIndicator'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/ProgressValidation/DuSpinner'),

        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Surfaces/DuCallout'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Surfaces/DuCoachmark'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Surfaces/DuDialog'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Surfaces/DuDialogFooter'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Surfaces/DuDocumentCard'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Surfaces/DuDocumentCardActions'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Surfaces/DuDocumentCardActivity'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Surfaces/DuDocumentCardLocation'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Surfaces/DuDocumentCardPreview'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Surfaces/DuDocumentCardTitle'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Surfaces/DuHoverCard'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Surfaces/DuTeachingBubble'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Surfaces/DuTeachingBubbleContent'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Surfaces/DuToolTip'),
        PLATFORM.moduleName('@dunite/au-office-ui/resources/elements/Surfaces/DuPanel')
    ]);
}
