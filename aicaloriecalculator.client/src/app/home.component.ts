import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html'
})
export class HomeComponent implements OnInit {
  public forecasts: WeatherForecast[] = [];
  public isLoggedIn = false;
  public errorMsg = '';

  constructor(private http: HttpClient, private router: Router) {
    // Listen for navigation changes to update login state
    this.router.events.pipe(filter(e => e instanceof NavigationEnd)).subscribe(() => {
      this.checkLogin();
    });
  }

  ngOnInit() {
    this.checkLogin();
  }

  checkLogin() {
    const token = localStorage.getItem('token');
    this.isLoggedIn = !!token;
    if (this.isLoggedIn) {
      this.getForecasts(token!);
    } else {
      this.forecasts = [];
      this.errorMsg = 'Please login to view the weather forecast.';
    }
  }

  getForecasts(token: string) {
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
    this.http.get<WeatherForecast[]>('/weatherforecast', { headers }).subscribe(
      (result) => {
        this.forecasts = result;
        this.errorMsg = '';
      },
      (error) => {
        this.errorMsg = 'Unable to fetch weather data. Please login.';
        if (error.status === 401) {
          localStorage.removeItem('token');
          this.isLoggedIn = false;
          this.forecasts = [];
        }
      }
    );
  }
}
