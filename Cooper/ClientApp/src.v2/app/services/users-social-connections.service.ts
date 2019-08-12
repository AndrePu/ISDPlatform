import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { User } from '@models';
import { timeout, catchError } from 'rxjs/operators';


const userSocialConnectionUrl = '/socialConnections';
  
@Injectable({
  providedIn: 'root'
})
export class UsersSocialConnectionsService {


  constructor(private http: HttpClient) { }

  async getFriends(userId: number): Promise<User[]> {

    const response = await this.http.get<User[]>(userSocialConnectionUrl + '/friends/' + userId.toString()).pipe(timeout(3000),
    catchError(e => {
      console.log('Too long respond for taking friends for user whose Id =' + userId.toString());

      // do smth then
      return of(null);
    })
    ).toPromise();

    return response;
  }

  async getSubscribers(userId: number): Promise<User[]> {

    const response = await this.http.get<User[]>(userSocialConnectionUrl + '/subscribers/' + userId.toString()).pipe(timeout(3000),
    catchError(e => {
      console.log('Too long respond for taking subscribers for user whose Id =' + userId.toString());

      // do smth then
      return of(null);
    })
    ).toPromise();

    return response;
  }

  async getSubscriptions(userId: number): Promise<User[]> {

    const response = await this.http.get<User[]>(userSocialConnectionUrl + '/subscriptions/' + userId.toString()).pipe(timeout(3000),
    catchError(e => {
      console.log('Too long respond for taking subscriptions for user whose Id =' + userId.toString());

      // do smth then
      return of(null);
    })
    ).toPromise();

    return response;

  }

  async getBlacklist(): Promise<User[]> {

    const response = await this.http.get<User[]>(userSocialConnectionUrl + '/blacklist').pipe(timeout(3000),
    catchError(e => {
      console.log('Too long respond for taking blacklist for currect session user');

      // do smth then
      return of(null);
    })
    ).toPromise();

    return response;
  }
}
