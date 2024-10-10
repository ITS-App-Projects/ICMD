import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HighlightModule, HIGHLIGHT_OPTIONS } from 'ngx-highlightjs';
import { NgbNavModule, NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { NoticeComponent } from './notice/notice.component';
import { CodePreviewComponent } from './code-preview/code-preview.component';
import { CoreModule } from '../../../core';
import { NgScrollbarModule } from 'ngx-scrollbar';

@NgModule({
  declarations: [NoticeComponent, CodePreviewComponent],
  imports: [
    CommonModule,
    CoreModule,
    HighlightModule,
    NgScrollbarModule,
    // ngbootstrap
    NgbNavModule,
    NgbTooltipModule,
    InlineSVGModule,
  ],
  exports: [NoticeComponent, CodePreviewComponent],
})
export class GeneralModule {}
