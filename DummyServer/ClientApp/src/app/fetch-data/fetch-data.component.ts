import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {Subject} from "rxjs";
import {switchMap, startWith} from "rxjs/operators";


@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
  public forecasts: WeatherForecast[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {

    this.clickStream.asObservable().pipe(
      startWith(undefined),
      switchMap(() => http.get<WeatherForecast[]>(baseUrl + 'weatherforecast'))
    ).subscribe(result => {
      this.forecasts = result;
      }, error => console.error(error));
  }
  
  private clickStream = new Subject<Event>();

  public refresh() {
    this.clickStream.next();
  }

}

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}
