import { Component } from '@angular/core';
import { CoopNavBarItem } from '@models';

@Component({
  selector: 'coop-top-panel-layout',
  templateUrl: './top-panel.layout.html',
  styleUrls: ['./top-panel.layout.scss']
})
export class TopPanelLayoutComponent {

  public navigationItems: CoopNavBarItem[] = [
    {label: 'Home', link: '/platform/home'},
    {label: 'Games', link: '/platform/games'},
    {label: 'Chats', link: '/platform/chats'},
    {label: 'My profile', link: '#'},
    {label: 'Forum', link: '#'},
    {label: 'Vacancies', link: '#'}
  ];

}
