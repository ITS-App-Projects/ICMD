import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { SearchDropdownInnerComponent } from './dropdown-inner/search-dropdown-inner/search-dropdown-inner.component';
import { NotificationsDropdownInnerComponent } from './dropdown-inner/notifications-dropdown-inner/notifications-dropdown-inner.component';
import { QuickActionsDropdownInnerComponent } from './dropdown-inner/quick-actions-dropdown-inner/quick-actions-dropdown-inner.component';
import { CartDropdownInnerComponent } from './dropdown-inner/cart-dropdown-inner/cart-dropdown-inner.component';
import { UserDropdownInnerComponent } from './dropdown-inner/user-dropdown-inner/user-dropdown-inner.component';
import { SearchOffcanvasComponent } from './offcanvas/search-offcanvas/search-offcanvas.component';
import { SearchResultComponent } from './dropdown-inner/search-dropdown-inner/search-result/search-result.component';
import { NotificationsOffcanvasComponent } from './offcanvas/notifications-offcanvas/notifications-offcanvas.component';
import { QuickActionsOffcanvasComponent } from './offcanvas/quick-actions-offcanvas/quick-actions-offcanvas.component';
import { CartOffcanvasComponent } from './offcanvas/cart-offcanvas/cart-offcanvas.component';
import { QuickPanelOffcanvasComponent } from './offcanvas/quick-panel-offcanvas/quick-panel-offcanvas.component';
import { UserOffcanvasComponent } from './offcanvas/user-offcanvas/user-offcanvas.component';
import { CoreModule } from '../../../core';
import { ScrollTopComponent } from './scroll-top/scroll-top.component';
import { ToolbarComponent } from './toolbar/toolbar.component';
import { NgScrollbarModule } from 'ngx-scrollbar';

@NgModule({
  declarations: [
    SearchDropdownInnerComponent,
    NotificationsDropdownInnerComponent,
    QuickActionsDropdownInnerComponent,
    CartDropdownInnerComponent,
    UserDropdownInnerComponent,
    SearchOffcanvasComponent,
    SearchResultComponent,
    NotificationsOffcanvasComponent,
    QuickActionsOffcanvasComponent,
    CartOffcanvasComponent,
    QuickPanelOffcanvasComponent,
    UserOffcanvasComponent,
    ScrollTopComponent,
    ToolbarComponent,
  ],
  imports: [CommonModule, InlineSVGModule, NgScrollbarModule, CoreModule, RouterModule],
  providers: [
  ],
  exports: [
    SearchDropdownInnerComponent,
    NotificationsDropdownInnerComponent,
    QuickActionsDropdownInnerComponent,
    CartDropdownInnerComponent,
    UserDropdownInnerComponent,
    SearchOffcanvasComponent,
    NotificationsOffcanvasComponent,
    QuickActionsOffcanvasComponent,
    CartOffcanvasComponent,
    QuickPanelOffcanvasComponent,
    UserOffcanvasComponent,
    ToolbarComponent,
    ScrollTopComponent,
  ],
})
export class ExtrasModule { }
