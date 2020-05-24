<template>
    <main class="main">
        <router-link to="/" class="back-to-index"><button class="back-to-index-btn">На главную</button></router-link>
        <yandex-map :show-all-markers="aggregateResult.pharmacies.length > 1" 
                    style="height: 100%" 
                    :controls="[]" 
                    :zoom="16" 
                    :scrollZoom="true" 
                    :coords="aggregateResult.pharmacies.length === 1 ? [aggregateResult.pharmacies[0].latitude, aggregateResult.pharmacies[0].longitude] : coords" 
                    :settings="mapSettings" 
        >
            <ymap-marker v-for="pharmacy in aggregateResult.pharmacies" 
                         :key="pharmacy.id" 
                         :marker-id="pharmacy.id" 
                         :coords="[pharmacy.latitude,pharmacy.longitude]"
                         :balloon="{header: pharmacy.title, body: pharmacy.address, footer: pharmacy.medicaments.map(e => e.title).join('\r\n')}"
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
            'ymap-marker': ymapMarker
        },
        data: function() {
            return {
                mapSettings: GeoLocationProvider.getSettings(),
                coords: GeoLocationProvider.getCoords().split(',').map(parseFloat),
            }
        },
        props: {
            aggregateResult: Object
        }
    }
</script>

<style scoped>
    .main {
        margin: auto;
        width: 800px;
        height: 600px;
        /*display: grid;*/
        /*grid-template-rows: 50px 50px 300px;*/
        /*grid-template-columns: 1fr;*/
        /*grid-row-gap: 5px;*/
    }
    .back-to-index {
        position: absolute;
        z-index: 15001;
    }
    .back-to-index-btn {
        width: 100px;
        height: 40px;
        background-color: white;
    }
</style>