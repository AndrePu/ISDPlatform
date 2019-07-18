import { NgModule } from '@angular/core';
import { AppComponent } from './app.component';
import { BrowserModule } from '@angular/platform-browser';
import {HttpClientModule, HTTP_INTERCEPTORS} from '@angular/common/http';
import {FormsModule} from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { DynamiSocialLoginModule, AuthServiceConfig, GoogleLoginProvider, FacebookLoginProvider } from 'ng-dynami-social-login';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatInputModule, MatButtonModule, MatCardModule, MatListModule, MatTabsModule, MatBadgeModule} from '@angular/material';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatTableModule} from '@angular/material/table';
import { ReactiveFormsModule } from '@angular/forms';
import { CooperInterceptor } from 'src/assets/cooper.interceptor';
import { GrowlModule } from 'primeng/primeng';
import {MatIconModule} from '@angular/material/icon';


import { LoginLayoutComponent, GameLayoutComponent, GamesLayoutComponent,
  PlatformLayoutComponent, ProfileLayoutComponent, TopPanelLayoutComponent,
  RegistrationLayoutComponent, PageNotFoundLayoutComponent   } from './layouts';

import {LoginComponent, RegistrationComponent, PosterComponent} from './components';


export function getAuthServiceConfigs() {
  const config = new AuthServiceConfig(
      [
         {
          id: FacebookLoginProvider.PROVIDER_ID,
          provider: new FacebookLoginProvider('INSERT_FACEBOOK_APP_ID')
        },
        {
          id: GoogleLoginProvider.PROVIDER_ID,
          provider: new GoogleLoginProvider('INSERT_GOOGLE_APP_ID')
        }

      ]
  );
  return config;
}
@NgModule({
  declarations: [
    AppComponent,
    LoginLayoutComponent,
    RegistrationLayoutComponent,
    GameLayoutComponent,
    GamesLayoutComponent,
    TopPanelLayoutComponent,
    ProfileLayoutComponent,
    PlatformLayoutComponent,
    PageNotFoundLayoutComponent,

  LoginComponent,
  RegistrationComponent,
  PosterComponent,
],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    MatInputModule,
    MatFormFieldModule,
    MatTableModule,
    MatButtonModule,
    MatCardModule,
    MatListModule,
    DynamiSocialLoginModule,
    GrowlModule,
    MatIconModule,
    MatTabsModule,
    MatBadgeModule
  ],
  providers: [
    {
      provide: AuthServiceConfig,
      useFactory: getAuthServiceConfigs
      },
      {
        provide: HTTP_INTERCEPTORS,
        useClass: CooperInterceptor,
        multi: true // give the possibility of various interceptors
      }
  ],
  bootstrap: [ AppComponent ]
})
export class AppModule { }
export class PizzaPartyAppModule { }
export class InputOverviewExample {}

