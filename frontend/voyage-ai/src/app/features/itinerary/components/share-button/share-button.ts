import { Component, EventEmitter, Output } from '@angular/core';

import { CommonModule } from '@angular/common';

import { MatButtonModule } from '@angular/material/button';

import { MatIconModule } from '@angular/material/icon';

@Component({

    selector: 'app-share-button',

    standalone: true,

    imports: [

        CommonModule,

        MatButtonModule,

        MatIconModule

    ],

    templateUrl: './share-button.html',

    styleUrl: './share-button.scss'

})

export class ShareButton {

    @Output()
    shareClicked = new EventEmitter<void>();

    @Output()
    exportPdfClicked = new EventEmitter<void>();

    share(): void {

        this.shareClicked.emit();

    }

    exportPdf(): void {

        this.exportPdfClicked.emit();

    }

}