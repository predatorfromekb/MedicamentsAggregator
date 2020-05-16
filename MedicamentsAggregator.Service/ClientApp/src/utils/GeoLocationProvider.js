import { loadYmap } from 'vue-yandex-maps';
const key = 'user_coords';

export default class GeoLocationProvider {
    static provide() {
        const value = window.localStorage[key];
        if (value != null) {
            const splitValue = value.split(',').map(parseFloat);
            return Promise.resolve(splitValue)
        }
        
        const mapSettings = {
            apiKey: '59348c34-7093-4200-a90f-1b518bf78da0',
            lang: 'ru_RU',
            coordorder: 'latlong',
            version: '2.1'
        };
        
        return loadYmap({ ...mapSettings, debug: true })
            .then(() => window.ymaps.geolocation.get({
                provider: 'browser',
                mapStateAutoApply: true
            }))
            .then((result) =>  {
                    // Получение местоположения пользователя.
                    const userCoordinates = result.geoObjects.get(0).geometry.getCoordinates();
                    window.localStorage[key] = userCoordinates;
                    console.log(userCoordinates);
                    return userCoordinates;
                })
            .catch(() => [56.838011, 60.597465]);
    }
}