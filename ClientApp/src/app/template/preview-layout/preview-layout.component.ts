import { Component, Input } from '@angular/core';
import { RendererType } from 'codezen-ui-layout';

@Component({
    selector: 'app-preview-layout',
    templateUrl: './preview-layout.component.html',
    styleUrl: './preview-layout.component.scss'
})
export class PreviewLayoutComponent {
    @Input() mappedData: any[] = [];
    public rendererType = RendererType.PREVIEW;
    public selectorPrefix = 'use-cz-';
}