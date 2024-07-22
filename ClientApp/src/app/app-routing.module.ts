import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { HomeComponent } from './home/home.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { TemplateComponent } from './template/template/template.component';
import { canActivateTeam } from './angular-app-services/permissions.service';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  // {
  //   path: '',
  //   canActivateChild: [canActivateTeam],
  //   loadChildren: () => HomeModule
  // },
  {
    path: '',
    component: HomeComponent,
    canActivateChild: [canActivateTeam],
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'dashboard'
      }, {
        path: 'dashboard',
        component: DashboardComponent
      },
      {
        path: ':entityName',
        component: TemplateComponent
      }
    ]
  },
  { path: '**', redirectTo: 'login' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
