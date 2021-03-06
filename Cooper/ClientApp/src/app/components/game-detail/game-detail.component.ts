import { Component, OnInit, Input, Pipe, PipeTransform } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { Game } from '../../models/game';
import { GameService }  from '../../services/game.service';

@Component({
  selector: 'cooper-game-detail',
  templateUrl: './game-detail.component.html',
  styleUrls: ['./game-detail.component.css']
})

export class GameDetailComponent implements OnInit {

  url: string;

  constructor(
    private route: ActivatedRoute,
    private gameService: GameService,
    private location: Location
  ) 
  {
    //this.url = this.game.link;    
  }


  @Input() game: Game;

  //sanitizer: DomSanitizer;

  ngOnInit() {
    //this.url = this.sanitizer.bypassSecurityTrustResourceUrl(this.game.link);  
    this.getGame();
  }

  getGame(): void {
    const id = +this.route.snapshot.paramMap.get('id');
    this.gameService.getGame(id)
      .subscribe(game => this.game = game);
  }

  goBack(): void {
    this.location.back();
  }

  save(): void {
    this.gameService.updateGame(this.game)
      .subscribe(() => this.goBack());
  }

}
