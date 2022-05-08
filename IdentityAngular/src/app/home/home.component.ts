import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth.service';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(private authService: AuthService, private httpClient: HttpClient) { }

  message: string;
  bankData: any = "Bağlantı sağlanamadı...";

  ngOnInit(): void {
    this.authService.userManager.getUser().then(user => {
      //login olduysa burası çalışacak
      if (user) {
        console.log(user);
        localStorage.setItem("accessToken", user.access_token);
        this.message = "Giriş başarılı";
      } else {
        this.message = "Griş başarısız";
      }
    }).then(() => this.httpClient.get("https://localhost:2000/api/weatherforecast/read", {
      headers:{"Authorization":"Bearer " + localStorage.getItem("accessToken")}
    }).subscribe(data => this.bankData = data));
  }

  login() {
    this.authService.userManager.signinRedirect();
  }

  logout() {
    this.authService.userManager.signoutRedirect();
  }

}
