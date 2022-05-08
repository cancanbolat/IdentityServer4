import { Injectable } from '@angular/core';
import * as oidc from 'oidc-client';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  config: oidc.UserManagerSettings = {
    authority: "https://localhost:1000",
    client_id: "AngularClient",
    redirect_uri: "http://localhost:4200/callback",
    post_logout_redirect_uri: "http://localhost:4200",
    response_type: "code",
    scope: "Garanti.Write Garanti.Read profile openid email Roles"
  };

  userManager: oidc.UserManager;

  constructor() {
    this.userManager = new oidc.UserManager(this.config);
  }
}
