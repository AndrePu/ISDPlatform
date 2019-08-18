import { NgModule } from '@angular/core';
import { AppComponent } from './app.component';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS, HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { DynamiSocialLoginModule, AuthServiceConfig, GoogleLoginProvider, FacebookLoginProvider } from 'ng-dynami-social-login';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import {
  MatInputModule, MatButtonModule, MatCardModule, MatListModule, MatTabsModule,
  MatBadgeModule, MatGridListModule, MatRippleModule
} from '@angular/material';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatTableModule } from '@angular/material/table';
import { ReactiveFormsModule } from '@angular/forms';
import { CooperInterceptor } from '@services';
import { GrowlModule } from 'primeng/primeng';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { AuthGuard } from '@guards';
import { SafePipe } from '@pipes';
import {
  LoginLayoutComponent, GameLayoutComponent, GamesLayoutComponent,
  PlatformLayoutComponent, ProfileLayoutComponent, TopPanelLayoutComponent,
  RegistrationLayoutComponent, PageNotFoundLayoutComponent, HomeLayoutComponent } from '@layouts';
import {
  LoginFormComponent, RegistrationFormComponent, PosterComponent, UserConnectionsListComponent,
  GameListItemComponent, GameCardComponent, NavigationComponent, UserInfoComponent, MyProfileComponent, GamesListComponent,
  AvatarCardComponent} from '@components';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { MatSelectModule } from '@angular/material/select';
import { LanguageSelectComponent } from './components/language-select/language-select.component';
import { LanguageNamePipe } from '@pipes';

export function getAuthServiceConfigs(): AuthServiceConfig {
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

export function HttpLoaderFactory(http: HttpClient): any {
  return new TranslateHttpLoader(http, '/assets/i18n/', '.json');
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
    HomeLayoutComponent,
    SafePipe,
    LoginFormComponent,
    RegistrationFormComponent,
    PosterComponent,
    UserConnectionsListComponent,
    GamesListComponent,
    UserInfoComponent,
    MyProfileComponent,
    GameListItemComponent,
    GameCardComponent,
    NavigationComponent,
    AvatarCardComponent,
    LanguageSelectComponent,
    LanguageNamePipe
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
    MatBadgeModule,
    MatGridListModule,
    MatRippleModule,
    MatProgressBarModule,
    MatButtonToggleModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient]
      }
    }),
    MatSelectModule
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
    },
    AuthGuard

  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
export class PizzaPartyAppModule { }
export class InputOverviewExample { }
