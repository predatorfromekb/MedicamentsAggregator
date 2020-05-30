<template>
    <main class="main">
        
        <div class="info" >
            <router-link :to="{ name: 'index', params: {selectedMedicaments: this.selectedMedicaments, settings: this.settings } }" class="back-to-index ">
                <h3>На главную</h3>
            </router-link>
            <h2>Всего - {{Math.round(aggregateResult.totalPrice*10)/10}}</h2>
            <div v-html="aggregateResult.coordinates.reduce((i,e) => i + renderPharmacies(e.pharmacies), '')">
                
            </div>
        </div>
        <yandex-map :show-all-markers="aggregateResult.coordinates.length > 1"
                    style="height: 100%"
                    :controls="[]"
                    :zoom="16"
                    :scrollZoom="true"
                    :coords="aggregateResult.coordinates.length === 1 ? [aggregateResult.coordinates[0].latitude, aggregateResult.coordinates[0].longitude] : coords"
                    :settings="mapSettings"
        >
            <ymap-marker v-for="(coordinate, index) in aggregateResult.coordinates"
                         :key="index"
                         :marker-id="index"
                         :coords="[coordinate.latitude,coordinate.longitude]"
                         :balloon="{
                                header: coordinate.address, 
                                body: renderPharmacies(coordinate.pharmacies)
                             }"

            />
        </yandex-map>
    </main>
</template>

<script>
    import { yandexMap, ymapMarker } from 'vue-yandex-maps';
    import GeoLocationProvider from "../utils/GeoLocationProvider";
    export default {
        name: "Result",
        components: {
            'yandex-map': yandexMap,
            'ymap-marker': ymapMarker,
        },
        data: function() {
            return {
                mapSettings: GeoLocationProvider.getSettings(),
                coords: GeoLocationProvider.getCoords(true).split(',').map(parseFloat),
            }
        },
        props: {
            aggregateResult: Object,
            selectedMedicaments: Map,
            settings: Object
        },
        methods: {
            renderPharmacies: function (pharmacies) {
                return pharmacies.reduce((p,pharmacy) => 
                    p + '<div style="margin: 5px; border-top: 1px #dddddd dotted">'
                    +'<div><strong>' + pharmacy.title + '</strong></div>' 
                    + pharmacy.medicaments.reduce((r,medicament)  => 
                        r + '<div style="margin: 5px">' 
                        + '<strong>'+ medicament.count + ' x '+ medicament.price + ' руб.</strong> '
                        + medicament.title 
                        + '</div>',
                    '') 
                    + '<div><strong>Итого - '+ Math.round((pharmacy.medicaments.map(e => e.price*e.count).reduce((c,a) => c + a, 0))*10)/10 +' руб</strong></div>'
                    + '</div>',
                '');
            }
        }
    }
</script>

<style scoped>
    .main {
        margin: auto;
        width: 800px;
        height: 600px;
        display: grid;
        grid-template-columns: 1fr 3fr;
    }
    .info {
        max-height: 600px;
        font-family: Consolas,serif;
        font-size: 10px;
        overflow: scroll;
    }
    .back-to-index {
        z-index: 15001;
    }
    .back-to-index-btn {
        width: 100px;
        height: 40px;
        background-color: white;
    }
</style>