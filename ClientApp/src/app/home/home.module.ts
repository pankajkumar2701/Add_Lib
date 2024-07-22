import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HomeRoutingModule } from './home-routing.module';
import { SideBarComponent } from './side-bar/side-bar.component';
import { DashboardComponent } from '../dashboard/dashboard.component';
import { LoginComponent } from '../login/login.component';
import { TopBarComponent } from './top-bar/top-bar.component';
import { HomeComponent } from './home.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TemplateListComponent } from '../template/tempale-list/template-list.component';
import { TemplateAddComponent } from '../template/template-add/template-add.component';
import { TemplateComponent } from '../template/template/template.component';
import { MatButtonModule } from '@angular/material/button';
import { provideNativeDateAdapter } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatDialogModule } from '@angular/material/dialog';
import { TemplatePreviewComponent } from '../template/template-preview/template-preview.component';
import { DynamicLayoutComponent } from '../template/dynamic-layout/dynamic-layout.component';
import { PreviewLayoutComponent } from '../template/preview-layout/preview-layout.component';
import { CodezenUILayoutModule } from 'codezen-ui-layout';
import { TooltipsModule } from '@progress/kendo-angular-tooltip';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { LabelModule } from '@progress/kendo-angular-label';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { ProgressBarModule } from '@progress/kendo-angular-progressbar';

@NgModule({
  declarations: [
    SideBarComponent,
    TopBarComponent,
    HomeComponent,
    LoginComponent,
    TemplateListComponent,
    TemplateAddComponent,
    DashboardComponent,
    TemplateComponent,
    TemplatePreviewComponent,
    DynamicLayoutComponent,
    PreviewLayoutComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    HomeRoutingModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatDialogModule,
    FormsModule,
    CodezenUILayoutModule,
    TooltipsModule,
    InputsModule,
    LayoutModule,
    LabelModule,
    ButtonsModule,
    ProgressBarModule
  ],
  providers: [
    provideNativeDateAdapter()
  ]
})
export class HomeModule { }
